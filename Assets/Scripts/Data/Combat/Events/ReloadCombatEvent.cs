using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabletopSimulator.Combat 
{ 
    public class ReloadCombatEvent : ACombatEvent
    {
        public ReloadCombatEvent(Creature src) : base(src) { }
        public override string GetText()
        {
            return $"{Source} reloaded their weapon.";
        }
    }
}
