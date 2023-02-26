using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo
{
    public string name;
    public string id;

    public float dmgMod;
    public float dmgScale;
    public float aimMod;
    public float aimScale;
    public float dtMod;
    public float dtScale;

    public Ammo(string id, string name, float v1, float v2, float v3): this(id, name, v1, 1.0f, v2, 1.0f, v3, 1.0f) {
    }

    public Ammo(string id, string name, float v1, float v2, float v3, float v4, float v5, float v6)
    {
        this.id = id;
        this.name = name;

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
    static Dictionary<string, Ammo> _instances = new Dictionary<string, Ammo>();
    // TODO remove this when json is fully implemented.
    static Dictionary<string, System.Func<Ammo>> build_funcs = new Dictionary<string, System.Func<Ammo>> {
        ["9mm"] = build9mm,
        ["556"] = build556
    };

    public static Ammo Get(string ammoID) {
        if (!_instances.ContainsKey(ammoID)) {
            _instances[ammoID] = build_funcs[ammoID]();
        }

        return _instances[ammoID];
    }

    public static Ammo build556()
    {
        return new Ammo("556", "556", 0.0f, 0.0f, -2.0f);
    }

    public static Ammo build9mm()
    {
        return new Ammo("9mm", "9mm", 0.0f, 0.0f, 0.0f);
    }
}