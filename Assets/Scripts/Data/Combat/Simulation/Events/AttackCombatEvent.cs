using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTRPGSimulator.Combat.Simulation
{
    public class AttackCombatEvent : ASimulationEvent
    {
        public int HitRoll { get; set; }
        public int DefenderAC { get; set; }
        public int DefenderHealth { get; set; }
        public int DamageDone { get; set; }

        public AttackCombatEvent(GameCharacter src, GameCharacter target) : this(src, target, 0, 0, 0) { }

        public AttackCombatEvent(GameCharacter src, GameCharacter target, int hit, int ac, int dmg) : base(src, target)
        {
            HitRoll = hit;
            DefenderAC = ac;
            DamageDone = dmg;
            DefenderHealth = target.Health;
        }

        public override string GetText()
        {
            string descript = $"{((GameCharacter)Source).Name} {(HitRoll > DefenderAC ? "hit" : "missed")} {((GameCharacter)Target).Name}!";
            string damageStr = $" | Damage Done: {DamageDone}";
            string hpStr = $" | HP Left: {DefenderHealth}";

            if (HitRoll >= DefenderAC)
                return descript + damageStr + hpStr;
            return descript + hpStr;
        }
    }
}