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

    public ARMOR_ID ID;

    public int AC { get; private set; }
    public int BDT { get; private set; }
    public int EDT { get; private set; }

    public int XDT { get; private set; }

    public Armor(ARMOR_ID id, int ac, int bdt, int edt, int xdt)
    {
        ID = id;
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
        return new Armor(Armor.ARMOR_ID.SCRAP, 8, 3, 1, 0);
    }

    private static Armor BuildLeatherJacket()
    {
        return new Armor(Armor.ARMOR_ID.JACKET, 12, 0, 0, 0);
    }

    private static Armor BuildFlak()
    {
        return new Armor(Armor.ARMOR_ID.FLAK, 12, 4, 4, 6);
    }

    private static Armor BuildReinforced()
    {
        return new Armor(Armor.ARMOR_ID.REINFORCED, 11, 3, 2, 2);
    }

    public static Armor Build(Armor.ARMOR_ID aid)
    {
        return builderMap[aid]();
    }

    public static Armor BuildLeather()
    {
        return new Armor(Armor.ARMOR_ID.LEATHER, 11, 1, 0, 1);
    }

    public static Armor BuildCombat()
    {
        return new Armor(Armor.ARMOR_ID.COMBAT, 14, 8, 6, 6);
    }
} 
