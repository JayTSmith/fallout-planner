using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabletopSimulator.Combat
{
    public class AttackCombatEvent : ACombatEvent
    {
        public  Creature Target { get; set; }
        public int HitRoll { get; set; }
        public int DamageDone { get; set; }

        public AttackCombatEvent(Creature src, Creature target) : this(src, target, 0, 0) { }

        public AttackCombatEvent(Creature src, Creature target, int hit, int dmg) : base(src)
        {
            Target = target;
            HitRoll = hit;
            DamageDone = dmg;
        }

        public override string GetText()
        {
            return $"{Source.name} {(HitRoll > Target.GameCharacter.AC ? "hit" : "miss")} {Target.name}! Damage done: {DamageDone}";
        }
    }
}