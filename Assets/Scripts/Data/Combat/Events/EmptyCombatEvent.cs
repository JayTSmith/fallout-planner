using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabletopSimulator.Combat
{
    public class EmptyCombatEvent : ACombatEvent
    {
        public EmptyCombatEvent(Creature src) : base(src) { }
        public override string GetText()
        {
            return $"{Source.name} did nothing.";
        }
    }
}