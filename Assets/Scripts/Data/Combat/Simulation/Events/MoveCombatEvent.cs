using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTRPGSimulator.Combat.Simulation
{
    public class MoveCombatEvent : ASimulationEvent
    {
        public Vector3Int Start { get; set; }
        public Vector3Int Goal { get; set; }

        public MoveCombatEvent(Creature src, Vector3Int start, Vector3Int goal) : base(src) 
        {
            Start = start;
            Goal = goal;
        }

        public override string GetText()
        {
            return $"{Source} moved to {Goal}.";
        }
    }
}