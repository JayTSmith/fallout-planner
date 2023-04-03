using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTRPGSimulator.Combat.Simulation
{
#nullable enable
    public abstract class ASimulationEvent
    {
        public object Source;
        public object? Target;
        public int timesHandled = 0;

        protected ASimulationEvent(object src) : this(src, null) {}
        protected ASimulationEvent(object src, object? target) {
            Source = src;
            Target = target;
           
        }

        public abstract string GetText();
    }
}
