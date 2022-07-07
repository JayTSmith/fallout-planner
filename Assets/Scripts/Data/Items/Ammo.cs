using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Ammo
{
    public float dmgMod;
    public float dmgScale;
    public float aimMod;
    public float aimScale;
    public float dtMod;
    public float dtScale;

    public Ammo(float v1, float v2, float v3): this(v1, 1.0f, v2, 1.0f, v3, 1.0f) {
    }

    public Ammo(float v1, float v2, float v3, float v4, float v5, float v6)
    {
        dmgMod = v1;
        dmgScale = v2;
        aimMod = v3;
        aimScale = v4;
        dtMod = v5;
        dtScale = v6;
    }
}

public enum AmmoID { 
    NINEMM,
    FIVEFIVESIX,
}

public static class AmmoFactory
{
    static Dictionary<AmmoID, Ammo> _instances = new Dictionary<AmmoID, Ammo>();
    static Dictionary<AmmoID, System.Func<Ammo>> build_funcs = new Dictionary<AmmoID, System.Func<Ammo>> {
        [AmmoID.NINEMM] = build9mm,
        [AmmoID.FIVEFIVESIX] = build556
    };

    public static Ammo Get(AmmoID ammoID) {
        if (!_instances.ContainsKey(ammoID)) {
            _instances[ammoID] = build_funcs[ammoID]();
        }

        return _instances[ammoID];
    }

    public static Ammo build556()
    {
        return new Ammo(0.0f, 0.0f, -2.0f);
    }

    public static Ammo build9mm()
    {
        return new Ammo(0.0f, 0.0f, 0.0f);
    }
}