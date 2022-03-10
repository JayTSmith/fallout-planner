using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageDetails
{
    public GameObject Source;
    public GameObject Target;
    public int Amount;

    DamageDetails(GameObject s, GameObject t, int a) 
    {
        Source = s;
        Target = t;
        Amount = a;
    }
}
