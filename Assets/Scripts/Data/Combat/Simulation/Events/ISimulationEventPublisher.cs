using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTRPGSimulator.Combat.Simulation {
    public delegate void SimluationEvent(ASimulationEvent combatEvent);
    public interface ISimulationEventPublisher
    {
        // Publish combat event
        public event Action<ASimulationEvent> PublishSimEvent;
    }
}
