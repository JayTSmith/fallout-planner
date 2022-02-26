using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Faction
{

    [System.Flags]
    public enum FactionInfo 
    {
        NONE = 0,
        TOWNEE = 1 << 1,
        BANDIT = 1 << 2,
        SWOLE = 1 << 3,
        PLAYER_PARTY = 1 << 4,
        CRITTER = 1 << 5,
    }
}
