using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TTRPGSimulator.Combat.Simulation;

namespace TTRPGSimulator.Controller
{
    struct CombatInfo 
    {
        public int DamageDealt;
        public int DamageReceived;
        public int Count;
        public double AvgHitsTillDeath;
        public int DeathCount;
    }

    public class InfoTracker : MonoBehaviour
    {
        int TotalSims { get; set; }
        int WonSims { get; set; }
        Dictionary<string, CombatInfo> CombatInfoDict { get; set; } = new Dictionary<string, CombatInfo>();
        Dictionary<int, int> HitCount = new Dictionary<int, int>();

        UIController  uiController;

        // Start is called before the first frame update
        void Start()
        {
            uiController = GetComponent<UIController>();

            BattleController[] battleConts = FindObjectsOfType<BattleController>();
            foreach (BattleController battleCont in battleConts) 
            {
                battleCont.PubSimulationEvent += HandleSimEvent;
            }
            
        }

        private void HandleSimEvent(ASimulationEvent simEvent) 
        {
            if (simEvent is SimulationStartEvent stEvent)
            {
                foreach (Creature creature in (stEvent.Source as BattleController).Entities)
                {
                    if (!CombatInfoDict.ContainsKey(creature.name)) CombatInfoDict.Add(creature.name, new CombatInfo());
                    CombatInfoDict.TryGetValue(creature.name, out CombatInfo combatInfo);
                    combatInfo.Count++;
                }
            }
            else if (simEvent is SimulationOverEvent soEvent)
            {
                TotalSims++;
                if ((soEvent.WinningFaction & Faction.FactionInfo.PLAYER_PARTY) != 0) WonSims++;

                uiController?.SetSimulationCount(TotalSims);
                uiController?.SetSimulationChance((double)WonSims / TotalSims);

                uiController.AddSimulation($"Simulation #{TotalSims}", soEvent.Events);

                HitCount.Clear();
            }
            else if (simEvent is AttackCombatEvent atkEvent)
            {
                GameCharacter atkChar = atkEvent.Source as GameCharacter;
                GameCharacter defChar = atkEvent.Target as GameCharacter;

                CombatInfoDict.TryGetValue(atkChar.Name, out CombatInfo atkInfo);
                CombatInfoDict.TryGetValue(defChar.Name, out CombatInfo defInfo);

                atkInfo.DamageDealt += atkEvent.DamageDone;
                defInfo.DamageReceived += atkEvent.DamageDone;

                if (atkEvent.DamageDone > 0)
                {
                    if (!HitCount.ContainsKey(defChar.GetHashCode())) HitCount.Add(defChar.GetHashCode(), 0);
                    HitCount[defChar.GetHashCode()]++;

                    if (defChar.Health < 0)
                    {
                        // This is a mess. Probably worth actually branching this out.
                        defInfo.AvgHitsTillDeath = (defInfo.AvgHitsTillDeath * defInfo.DeathCount + HitCount[defChar.GetHashCode()]) / ++defInfo.DeathCount;
                    }
                }
            }
        }

    }
}