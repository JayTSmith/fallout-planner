using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabletopSimulator.Combat
{
    public class MoveCombatEvent : ACombatEvent
    {
        public Vector3Int Goal { get; set; }

        public MoveCombatEvent(Creature src, Vector3Int goal) : base(src) 
        {
            Goal = goal;
        }

        public override string GetText()
        {
            return $"{Source} moved to {Goal}.";
        }
    }
}