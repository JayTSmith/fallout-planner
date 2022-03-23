using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabletopSimulator.Combat
{
    public abstract class ACombatEvent
    {
        public Creature Source;

        protected ACombatEvent(Creature src) {
            Source = src;
        }

        public abstract string GetText();
    }
}
