using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTRPGSimulator.Combat.Simulation
{
    public class AttackCombatEvent : ASimulationEvent
    {
        public int HitRoll { get; set; }
        public int DefenderAC { get; set; }
        public int DamageDone { get; set; }

        public AttackCombatEvent(GameCharacter src, GameCharacter target) : this(src, target, 0, 0, 0) { }

        public AttackCombatEvent(GameCharacter src, GameCharacter target, int hit, int ac, int dmg) : base(src, target)
        {
            HitRoll = hit;
            DefenderAC = ac;
            DamageDone = dmg;
        }

        public override string GetText()
        {
            return $"{((GameCharacter) Source).Name} {(HitRoll > ((GameCharacter) Target).AC ? "hit" : "missed")} {((GameCharacter) Target).Name}! Damage done: {DamageDone} | HP Left: {((GameCharacter) Target).Health}";
        }
    }
}