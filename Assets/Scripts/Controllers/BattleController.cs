using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    enum BattleState 
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
    public List<Creature> entities { get; private set; } = new List<Creature>();
    private int currentIdx = -1;
    [SerializeField]
    private float timeBetweenTurns;
    private float timeWaited;

    [SerializeField]
    private GroundController groundController;

    [SerializeField]
    private bool triggerCombatOnStart = false;
    private BattleState battleState = BattleState.NOTSTARTED;
    public GameObject CurrentPlayer 
    {
        get 
        {
            {
                if (currentIdx < entities.Count)
                {
                    return entities[currentIdx].gameObject;
                }
                return null;
            }
        }
            
    }
    public bool IsCombat { get => battleState != BattleState.NOTSTARTED && battleState != BattleState.REFRESHING; }
    public int roundNum { get; private set; }


    public void Attack(Creature attacker, Creature defender) {
        GameCharacter atkChar = attacker.GameCharacter;
        GameCharacter defChar = defender.GameCharacter;
        Color lineColor = new Vector4(0,0,0,0);

        int rawAtkRoll = Die.makeDie(20).Roll();
        int atkRoll = rawAtkRoll + atkChar[atkChar.EquippedWeapon.WeaponSkill];
        int defAC = defChar.AC + defender.CoverBonus(attacker.gameObject.transform.position);

        int distance = (int) Math.Ceiling(Vector3Int.Distance(attacker.CurrentCell, defender.CurrentCell));
        if (distance < 0) 
        {
            distance *= -1;
        }
        
        int aimPenalty = 0;
        if (atkChar.EquippedWeapon.WeaponType != WeaponType.UNARMED && atkChar.EquippedWeapon.WeaponType !=WeaponType.MELEE)
        {
            if (!groundController.HasLineOfSight(attacker, defender))
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
            potentialDmg = (int) ((rawDmg + atkChar.GetBonusDamage()) * atkChar.EquippedWeapon.Ammo.dmgMod) - defChar.GetDT(atkChar.EquippedWeapon.DamageType);
        else
            potentialDmg = (int)((rawDmg + atkChar.GetBonusDamage()) + atkChar.EquippedWeapon.Ammo.dmgMod) - defChar.GetDT(atkChar.EquippedWeapon.DamageType);
        potentialDmg = Math.Max(potentialDmg, 0);

        int effectiveDT;
        if (atkChar.EquippedWeapon.Ammo.dtMult)
            effectiveDT = defChar.GetDT(atkChar.EquippedWeapon.DamageType) + (int)atkChar.EquippedWeapon.Ammo.dtMod;
        else
            effectiveDT = (int) (defChar.GetDT(atkChar.EquippedWeapon.DamageType) * atkChar.EquippedWeapon.Ammo.dtMod);
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
            lineColor = new Vector4(255 * (1 - (defChar.Health / ((float) defChar.MaxHealth()))), 0, 0, 255);
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
        if (entities.Contains(c)) 
        {
            entities.Remove(c);
            if (currentIdx >= entities.Count)
            {
                RoundEnd();
            }

        }
    }

    private void RoundStart()
    {
        Debug.Log($"Round {++roundNum} is starting!");
    }

    private void RoundEnd()
    {
        Debug.Log($"Round {roundNum} is over!");
        currentIdx = 0;
    }

    public void RefreshStatus() 
    {
        entities.Clear();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Being"))
        {
            if (go.activeInHierarchy)
                entities.Add(go.GetComponent<Creature>());
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (go.activeInHierarchy)
                entities.Add(go.GetComponent<Creature>());
        }
        
        currentIdx = 0;
        roundNum = 0;
        battleState = BattleState.NOTSTARTED;
        RollInitiatives();
    }

    private void RollInitiatives()
    {
        foreach (Creature cc in entities)
        {
            cc.GameCharacter.RollInitiative();
        }

        entities.Sort((Creature lhs, Creature rhs) => rhs.GameCharacter.Initiative.CompareTo(lhs.GameCharacter.Initiative));
    }

    // Start is called before the first frame update
    void Start()
    {
        if (triggerCombatOnStart)
        {
            roundNum = 0;
            RefreshStatus();
            battleState = BattleState.TURNSTART;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6) && !IsCombat)
        {
            battleState = BattleState.TURNSTART;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SpawnPoint[] spawners = FindObjectsOfType<SpawnPoint>();


            foreach (SpawnPoint spawner in spawners)
            {
                spawner.DespawnAll();
            }

            foreach (SpawnPoint spawner in spawners)
            {
                spawner.Spawn();
            }

            RefreshStatus();
        }

        if (entities.Count == 0)
            battleState = BattleState.NOTSTARTED;

        if (IsCombat)
        {
            AIController ac = entities[currentIdx].GetComponent<AIController>();
            PlayerController pc = entities[currentIdx].GetComponent<PlayerController>();
            if (battleState == BattleState.TURNSTART)
            {
                if (currentIdx == 0)
                    RoundStart();
                Creature curCreature = entities[currentIdx];
                curCreature.GameCharacter.CurrentMovePoints = curCreature.GameCharacter.MaxMovePoints;

                if (ac != null)
                {
                    StartCoroutine(ac.CombatStrategy.DoMovement());
                    battleState = BattleState.AIMOVING;
                }

                if (entities[currentIdx].GetComponent<PlayerController>() != null)
                    battleState = BattleState.PLAYERTURN;
            }

            else if (battleState == BattleState.AIMOVING)
            {
                if (!entities[currentIdx].IsMoving)
                {
                    battleState = BattleState.AIATTACK;
                }
            }
            else if (battleState == BattleState.AIATTACK)
            {
                ac.CombatStrategy.DoCombatAction();
                battleState = BattleState.AISUPPORT;
            }
            else if (battleState == BattleState.AISUPPORT)
            {
                ac.CombatStrategy.DoSupportAction();
                battleState = BattleState.AIBONUS;
            }
            else if (battleState == BattleState.AIBONUS)
            {
                ac.CombatStrategy.DoBonusAction();
                battleState = BattleState.TURNEND;
            }
            else if (battleState == BattleState.TURNEND)
            {
                if (timeWaited > timeBetweenTurns)
                {
                    timeWaited = 0f;
                    bool continueCombat = false;
                    bool roundEnded = currentIdx + 1 >= entities.Count;

                    Faction.FactionInfo faction = entities[currentIdx].GameCharacter.Faction;
                    foreach (Creature c in entities)
                    {
                        if ((c.GameCharacter.Faction & faction) == 0)
                        {
                            continueCombat = true;
                            break;
                        }
                    }

                    currentIdx++;

                    if (continueCombat)
                        battleState = BattleState.TURNSTART;
                    else
                    {
                        battleState = BattleState.NOTSTARTED;
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
