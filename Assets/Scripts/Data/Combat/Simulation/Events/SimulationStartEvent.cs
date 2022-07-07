using System.Collections;
using System.Collections.Generic;
using TTRPGSimulator.Controller;
using UnityEngine;


namespace TTRPGSimulator.Combat.Simulation
{
    public class SimulationStartEvent : ASimulationEvent
    {
        public SimulationStartEvent(BattleController src) : base(src) { }
        public override string GetText()
        {
            return $"New simulation started! ({(Source as BattleController).GetHashCode()})";
        }
    }
}