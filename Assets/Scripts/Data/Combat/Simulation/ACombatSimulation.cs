using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabletopSimulator.Combat.Simulation {
    public abstract class ACombatSimulation
    {
        private System.Random random = new System.Random();

        public abstract List<ACombatEvent> Run();
        public abstract void Reset();

        public System.Random GetRandom() { return random; }
    }
}