using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Ammo
{
    public float dmgMod;
    public float aimMod;
    public float dtMod;
    public bool dtMult;
    public bool dmgMult;

    public Ammo(float v1, float v2, float v3, bool v4 = false, bool v5 = false)
    {
        dmgMod = v1;
        dmgMult = v5;
        aimMod = v2;
        dtMod = v3;
        dtMult = v4;
    }
}

public static class AmmoFactory 
{
    public static Ammo build556()
    {
        return new Ammo(0.0f, 0.0f, -2.0f);
    }

    public static Ammo build9mm()
    {
        return new Ammo(0.0f, 0.0f, 0.0f);
    }
}