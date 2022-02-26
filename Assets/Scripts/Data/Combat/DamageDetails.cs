using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageDetails
{
    public GameObject Source;
    public int Amount;

    DamageDetails(GameObject s, int a) 
    {
        Source = s;
        Amount = a;
    }
}
