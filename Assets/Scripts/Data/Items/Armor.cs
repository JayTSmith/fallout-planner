using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor
{
    public enum ARMOR_ID
    {
        LEATHER,
        REINFORCED,
        FLAK,
        COMBAT,
        SCRAP,
        JACKET,
        NULL,
    }

    public string Name { get; private set; }
    public string ID { get; private set; }

    public int AC { get; private set; }
    public int BDT { get; private set; }
    public int EDT { get; private set; }

    public int XDT { get; private set; }

    public Armor(string id, string name, int ac, int bdt, int edt, int xdt)
    {
        ID = id;
        Name = name; 
        AC = ac;
        BDT = bdt;
        EDT = edt;
        XDT = xdt;
    }
}

public static class ArmorFactory
{
    private static Dictionary<Armor.ARMOR_ID, System.Func<Armor>> builderMap = new Dictionary<Armor.ARMOR_ID, System.Func<Armor>>
    {
        [Armor.ARMOR_ID.JACKET] = BuildLeatherJacket,
        [Armor.ARMOR_ID.SCRAP] = BuildScrap,
        [Armor.ARMOR_ID.LEATHER] = BuildLeather,
        [Armor.ARMOR_ID.REINFORCED] = BuildReinforced,
        [Armor.ARMOR_ID.FLAK] = BuildFlak,
        [Armor.ARMOR_ID.COMBAT] = BuildCombat,
    };

    private static Armor BuildScrap()
    {
        return new Armor("scrap", "scrap", 8, 3, 1, 0);
    }

    private static Armor BuildLeatherJacket()
    {
        return new Armor("leatherjacket", "scrap", 12, 0, 0, 0);
    }

    private static Armor BuildFlak()
    {
        return new Armor("flakjacket", "scrap", 12, 4, 4, 6);
    }

    private static Armor BuildReinforced()
    {
        return new Armor("leatherreinforced", "scrap", 11, 3, 2, 2);
    }

    public static Armor Build(Armor.ARMOR_ID aid)
    {
        if (!builderMap.ContainsKey(aid)) return null;
        return builderMap[aid]();
    }

    public static Armor BuildLeather()
    {
        return new Armor("leather", "scrap", 11, 1, 0, 1);
    }

    public static Armor BuildCombat()
    {
        return new Armor("combatbasic", "scrap", 14, 8, 6, 6);
    }
} 
