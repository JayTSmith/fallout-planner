using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TabletopSimulator.Combat {
    public delegate void CombatEvent(ACombatEvent combatEvent);
    public interface ICombatEventPublisher
    {
        // Publish combat event
        public event CombatEvent PubCombatEvent;
    }
}
