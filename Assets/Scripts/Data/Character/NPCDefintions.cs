using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPCDefintions
{
    public enum NPC_ID 
    { 
        BANDIT,
        GHOUL,
        BIGGHOUL,
        KENZIE,
        PEYTON,
        STEVEN,
        MICHAEL,
        CHARLIE,
    }

    public static Dictionary<NPC_ID, Color> NPCColors { get; private set; } = new Dictionary<NPC_ID, Color>()
    {
        [NPC_ID.BANDIT] = Color.red,
        [NPC_ID.PEYTON] = Color.green,
        [NPC_ID.MICHAEL] = Color.cyan,
        [NPC_ID.CHARLIE] = Color.blue,
        [NPC_ID.GHOUL] = new Vector4(.6f, .3f, 0f, 1.0f),
        [NPC_ID.BIGGHOUL] = new Vector4(.3f, .9f, 0.0f, 1.0f),
    };
}
 