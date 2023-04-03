using System.Collections;
using System.Collections.Generic;
using TTRPGSimulator.Combat.Simulation;
using UnityEngine;

public class RoundChangeEvent : ASimulationEvent
{
    public RoundChangeEvent(object src) : base(src)
    {
    }

    public RoundChangeEvent(object src, object target) : base(src, target)
    {
    }

    public override string GetText()
    {
        return "Round Changed";
    }
}
