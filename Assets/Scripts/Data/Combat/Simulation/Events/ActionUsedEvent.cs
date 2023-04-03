using System;

namespace TTRPGSimulator.Combat.Simulation
{
    public class ActionUsedEvent : ASimulationEvent
    {
        public enum ActionType { 
            None,
            Combat,
            Support,
            Movement,
        }

        public ActionType type;

        public ActionUsedEvent(object src, ActionType actionType) : base(src) => type = actionType;

        public override string GetText()
        {
            return $"{this.Source} used a {type} action.";
        }
    }
}
