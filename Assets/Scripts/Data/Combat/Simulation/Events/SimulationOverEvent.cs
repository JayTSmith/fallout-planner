using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTRPGSimulator.Combat.Simulation {
    public class SimulationOverEvent : ASimulationEvent
    {
        public IList<ASimulationEvent> Events { get; private set; }
        public Faction.FactionInfo WinningFaction { get; private set; }
        public SimulationOverEvent(Object src, Faction.FactionInfo faction, IList<ASimulationEvent> events) : base(src) {
            WinningFaction = faction;
            Events = events;
        }

        public override string GetText()
        {
            return $"Players {((WinningFaction & Faction.FactionInfo.PLAYER_PARTY) > 0 ? "won" : "lost")}";
        }
    }
}
