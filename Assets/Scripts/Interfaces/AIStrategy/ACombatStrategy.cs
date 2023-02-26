using System.Collections;
using System.Collections.Generic;
using TTRPGSimulator.Combat.Simulation;
using TTRPGSimulator.Controller;
using UnityEngine;

namespace TTRPGSimulator.AI
{
    public abstract class ACombatStrategy
    {
        public GameObject Self { get; set; }
        public bool IsBusy { get; set; }
        protected AIController AIController { get => Self.GetComponent<AIController>(); }
        protected BattleController BattleController { get => AIController.BattleController; }
        protected Creature Creature { get => Self.GetComponent<Creature>(); }
        protected GroundController GroundController { get => BattleController.GroundController; }

        public virtual void HandleSimulationEvent(ASimulationEvent simEvent) {
            Debug.Log(simEvent.GetType()); 
        }

        // Returns the goal position of the current movement. BattleController will adjust based on movement points.
        public virtual IEnumerator DoMovement()
        {
            yield break;
        }
        // Combat actions are known; this method assumes that the strategy is fair (aka that the action performed is a combat action.)
        public virtual void DoCombatAction()
        {
        }
        // This is a method for an AI to do anything extra.
        public virtual void DoBonusAction()
        {
        }
        // Support actions are known; this method assumes that the strategy is fair (aka that the action performed is a support action).
        public virtual void DoSupportAction()
        {
        }
    }
}