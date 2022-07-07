using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTRPGSimulator.Combat.Simulation
{
    public class EmptyCombatEvent : ASimulationEvent
    {
        public EmptyCombatEvent(Creature src) : base(src) { }
        public override string GetText()
        {
            return $"{(Source as Creature).name} did nothing.";
        }
    }
}