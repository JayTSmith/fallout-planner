using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using TabletopSimulator.Utilites;
using TabletopSimulator.Combat;

namespace TabletopSimulator.Controller
{
    public class BattleController : MonoBehaviour, ICombatEventPublisher
    {
        enum BattleStates
        {
            NOTSTARTED,
            TURNSTART,
            AIMOVING,
            AITURN,
            AIATTACK,
            AISUPPORT,
            AIBONUS,
            PLAYERTURN,
            PLAYERMOVING,
            TURNEND,
            REFRESHING
        }
        public List<Creature> Entities { get; private set; } = new List<Creature>();
        private int currentIdx = -1;
        [SerializeField]
        private float timeBetweenTurns;
        private float timeWaited;
        public GroundController GroundController;
        public bool simMode;

        private BattleStates BattleState = BattleStates.NOTSTARTED;


        public event CombatEvent PubCombatEvent;

        public GameObject CurrentPlayer
        {
            get
            {
                {
                    if (currentIdx < Entities.Count)
                    {
                        return Entities[currentIdx].gameObject;
                    }
                    return null;
                }
            }

        }
        public bool IsCombat { get => BattleState != BattleStates.NOTSTARTED && BattleState != BattleStates.REFRESHING; }
        public int RoundNum { get; private set; }

        // Movement Related Properties
        private float baseMoveSpeed = .03f;
        private Vector3 moveSpeed;
        private float moveTime;
        private List<Vector3Int> curPath = null;
        private int curNode;


        public void Attack(Creature attacker, Creature defender)
        {
            GameCharacter atkChar = attacker.GameCharacter;
            GameCharacter defChar = defender.GameCharacter;
            Color lineColor = new Vector4(0, 0, 0, 0);

            int rawAtkRoll = Die.makeDie(20).Roll();
            int atkRoll = rawAtkRoll + atkChar[atkChar.EquippedWeapon.WeaponSkill];
            int defAC = defChar.AC + defender.CoverBonus(attacker.gameObject.transform.position);

            int distance = (int)Math.Ceiling(Vector3Int.Distance(attacker.CurrentCell, defender.CurrentCell));
            if (distance < 0)
            {
                distance *= -1;
            }

            int aimPenalty = 0;
            if (atkChar.EquippedWeapon.WeaponType != WeaponType.UNARMED && atkChar.EquippedWeapon.WeaponType != WeaponType.MELEE)
            {
                if (!GroundController.HasLineOfSight(attacker, defender))
                {
                    Debug.Log($"{atkChar.Name} cannot see their target, {defChar.Name}.", attacker);
                    return;
                }

                int dist_divisor = 1;

                aimPenalty += distance / dist_divisor;
                aimPenalty -= atkChar.EquippedWeapon.Range;
                aimPenalty -= atkChar.GetSpecialMod(GameCharacter.SPECIAL.PERCEPTION);
                if (aimPenalty < 0)
                {
                    aimPenalty = 0;
                }
            }
            else if (distance > atkChar.EquippedWeapon.Range)
            {
                Debug.Log($"{atkChar.Name} is too far away ({distance} hexes) from {defChar.Name} to make an attack!", attacker);
                return;
            }

            int rawDmg = atkChar.EquippedWeapon.DamageDice.Roll();
            int potentialDmg;
            if (atkChar.EquippedWeapon.Ammo.dmgMult)
                potentialDmg = (int)((rawDmg + atkChar.GetBonusDamage()) * atkChar.EquippedWeapon.Ammo.dmgMod) - defChar.GetDT(atkChar.EquippedWeapon.DamageType);
            else
                potentialDmg = (int)((rawDmg + atkChar.GetBonusDamage()) + atkChar.EquippedWeapon.Ammo.dmgMod) - defChar.GetDT(atkChar.EquippedWeapon.DamageType);
            potentialDmg = Math.Max(potentialDmg, 0);

            int effectiveDT;
            if (atkChar.EquippedWeapon.Ammo.dtMult)
                effectiveDT = defChar.GetDT(atkChar.EquippedWeapon.DamageType) + (int)atkChar.EquippedWeapon.Ammo.dtMod;
            else
                effectiveDT = (int)(defChar.GetDT(atkChar.EquippedWeapon.DamageType) * atkChar.EquippedWeapon.Ammo.dtMod);
            effectiveDT = Math.Max(effectiveDT, 0);
            // Chance to hit.
            atkRoll -= aimPenalty;
            if (atkRoll < defAC)
            {
                potentialDmg = 0;
                //return; // We failed to make an attack. Check for critical failure.
            }
            bool isDead = defender.TakeDamage(potentialDmg);
            // Check for critical success.

            if (potentialDmg == 0)
            {
                lineColor = Color.blue;
            }
            else
            {
                lineColor = new Vector4(255 * (1 - (defChar.Health / ((float)defChar.MaxHealth))), 0, 0, 255);
            }

            Debug.Log($"{atkChar.Name} -> {defChar.Name}" +
                      $" Attack Roll: {atkRoll} (Roll: {rawAtkRoll} + WS: {atkChar[atkChar.EquippedWeapon.WeaponSkill]} - Penalty: {aimPenalty})" +
                      $" against {defAC} (AC: {defChar.AC} + Cover:{defender.CoverBonus(attacker.gameObject.transform.position)}).", attacker);
            Debug.Log($"Damage done: {potentialDmg} [Roll: {rawDmg} x DMG Mult: {atkChar.EquippedWeapon.Ammo.dmgMod} + Bonus: {atkChar.GetBonusDamage()} - DT: {effectiveDT} = ({defChar.GetDT(atkChar.EquippedWeapon.DamageType)} + {atkChar.EquippedWeapon.Ammo.dtMod})]" +
                      $" HP Remaining: {defChar.Health}", defender);
            //Debug.DrawLine(attacker.transform.position, defender.transform.position, lineColor, 4.0f);

            if (isDead)
            {
                Kill(defender);
            }
        }

        void Kill(Creature c)
        {
            if (Entities.Contains(c))
            {
                Entities.Remove(c);
                if (currentIdx >= Entities.Count)
                {
                    RoundEnd();
                }

            }
        }

        private void RoundStart()
        {
            Debug.Log($"Round {++RoundNum} is starting!");
        }

        private void RoundEnd()
        {
            Debug.Log($"Round {RoundNum} is over!");
            PubCombatEvent?.Invoke(new EmptyCombatEvent(null));
            currentIdx = 0;
        }

        public void RefreshStatus()
        {
            foreach (Creature creature in GetComponentsInChildren<Creature>())
            {
                Entities.Add(creature);
                // PubCombatEvent += creature.GetComponent<AIController>().CombatStrategy.HandleCombatEvent;
            }

            currentIdx = 0;
            RoundNum = 0;
            RollInitiatives();
            BattleState = BattleStates.TURNSTART;

            curPath = null;
        }

        private void RollInitiatives()
        {
            foreach (Creature cc in Entities)
            {
                cc.GameCharacter.RollInitiative();
            }

            Entities.Sort((Creature lhs, Creature rhs) => rhs.GameCharacter.Initiative.CompareTo(lhs.GameCharacter.Initiative));
        }


        // Start is called before the first frame update
        void Start()
        {
            RoundNum = 0;
            BattleState = BattleStates.REFRESHING;
        }

        // Update is called once per frame
        void Update()
        {
            if (BattleState == BattleStates.REFRESHING)
            {
                RefreshStatus();
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                Grid grid = GroundController.ControlledGrid;
                SerializableDictionary<string, SerializableDictionary<Vector3Int, string>> data = new SerializableDictionary<string, SerializableDictionary<Vector3Int, string>>();

                foreach (TilemapRenderer renderer in GroundController.GetComponentsInChildren<TilemapRenderer>())
                {
                    Tilemap tilemap = renderer.GetComponent<Tilemap>();
                    data[renderer.name] = new SerializableDictionary<Vector3Int, string>();

                    foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
                    {
                        data[renderer.name][pos] = tilemap.GetTile(pos) != null ? tilemap.GetTile(pos).name : "null";
                    }

                }
                Debug.LogWarning(JsonUtility.ToJson(data));
            }

            if (Input.GetKeyDown(KeyCode.F5) || BattleState == BattleStates.NOTSTARTED)
            {
                SpawnPoint[] spawners = GetComponentsInChildren<SpawnPoint>();

                foreach (SpawnPoint spawner in spawners)
                {
                    spawner.Despawn();
                }

                foreach (SpawnPoint spawner in spawners)
                {
                    spawner.Spawn();
                }

                foreach (Creature creature in Entities)
                {
                    //PubCombatEvent -= creature.GetComponent<AIController>().CombatStrategy.HandleCombatEvent;
                }
                Entities.Clear();
                BattleState = BattleStates.REFRESHING;
            }

            if (curPath != null)
            {
                if (simMode)
                {
                    Entities[currentIdx].transform.position = GroundController.ControlledGrid.CellToWorld(curPath[curPath.Count - 1]);
                    curPath = null;
                }
                else 
                {
                    Vector3 goal = GroundController.ControlledGrid.CellToWorld(curPath[curNode]);
                    if (Entities[currentIdx].transform.position != goal)
                    {
                        if (moveTime == 0.0f)
                            Entities[currentIdx].transform.position = goal;
                        else
                            Entities[currentIdx].transform.position = Vector3.SmoothDamp(Entities[currentIdx].transform.position, goal, ref moveSpeed, moveTime);
                    }
                    else if (curNode + 1 < curPath.Count)
                    {
                        curNode++;
                    }
                    else
                    {
                        curPath = null;
                    }
                }
            }

            if (IsCombat)
            {
                AIController ac = Entities[currentIdx].GetComponent<AIController>();
                //PlayerController pc = Entities[currentIdx].GetComponent<PlayerController>();
                if (BattleState == BattleStates.TURNSTART)
                {
                    if (currentIdx == 0)
                        RoundStart();
                    Creature curCreature = Entities[currentIdx];
                    curCreature.GameCharacter.CurrentMovePoints = curCreature.GameCharacter.MaxMovePoints;

                    if (ac != null)
                    {
                        StartCoroutine(ac.CombatStrategy.DoMovement());
                        BattleState = BattleStates.AIMOVING;
                    }

                    //if (Entities[currentIdx].GetComponent<PlayerController>() != null)
                    //    BatteState = BattleStates.PLAYERTURN;
                }

                else if (BattleState == BattleStates.AIMOVING)
                {
                    if (curPath == null)
                    {
                        BattleState = BattleStates.AIATTACK;
                    }
                }
                else if (BattleState == BattleStates.AIATTACK)
                {
                    ac.CombatStrategy.DoCombatAction();
                    BattleState = BattleStates.AISUPPORT;
                }
                else if (BattleState == BattleStates.AISUPPORT)
                {
                    ac.CombatStrategy.DoSupportAction();
                    BattleState = BattleStates.AIBONUS;
                }
                else if (BattleState == BattleStates.AIBONUS)
                {
                    ac.CombatStrategy.DoBonusAction();
                    BattleState = BattleStates.TURNEND;
                }
                else if (BattleState == BattleStates.TURNEND)
                {
                    if (timeWaited > timeBetweenTurns)
                    {
                        timeWaited = 0f;
                        bool continueCombat = false;
                        bool roundEnded = currentIdx + 1 >= Entities.Count;

                        Faction.FactionInfo faction = Entities[currentIdx].GameCharacter.Faction;
                        foreach (Creature c in Entities)
                        {
                            if ((c.GameCharacter.Faction & faction) == 0)
                            {
                                continueCombat = true;
                                break;
                            }
                        }

                        currentIdx++;

                        if (continueCombat)
                            BattleState = BattleStates.TURNSTART;
                        else
                        {
                            BattleState = BattleStates.NOTSTARTED;
                        }

                        if (roundEnded || !continueCombat)
                        {
                            RoundEnd();
                        }
                    }
                    else
                    {
                        timeWaited += Time.deltaTime;
                    }
                }

            }
        }
    }
}