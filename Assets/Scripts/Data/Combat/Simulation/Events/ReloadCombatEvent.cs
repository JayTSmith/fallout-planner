using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTRPGSimulator.Combat.Simulation
{ 
    public class ReloadCombatEvent : ASimulationEvent
    {
        public ReloadCombatEvent(Creature src) : base(src) { }
        public override string GetText()
        {
            return $"{Source} reloaded their weapon.";
        }
    }
}
