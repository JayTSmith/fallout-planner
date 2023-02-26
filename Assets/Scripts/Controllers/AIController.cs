using System;
using System.Collections;
using System.Collections.Generic;
using TTRPGSimulator.AI;
using TTRPGSimulator.Combat.Simulation;
using UnityEngine;

namespace TTRPGSimulator.Controller 
{
    public class AIController : MonoBehaviour, ISimulationEventPublisher
    {
        public ACombatStrategy CombatStrategy { get; set; }

        public GroundController GroundController;
        public BattleController BattleController;

        public event Action<ASimulationEvent> PublishSimEvent;

        public Creature Creature { get; set; }
        // Start is called before the first frame update

        void Start()
        {
            Creature = GetComponent<Creature>();


            CombatStrategy = new RecklessStrategy(gameObject);
        }
    }
}