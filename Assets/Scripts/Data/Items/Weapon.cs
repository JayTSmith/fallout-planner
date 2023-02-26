using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum DamageType {
    KINETIC,
    ENERGY,
    EXPLOSIVE,
    DIRECT
}

[System.Flags]
public enum WeaponType {
    NONE,
    MELEE,
    UNARMED,
    BALLISTIC,
    ENERGY,
    EXPLOSIVE,
    THROWN,
    SPECIAL
}

public enum WeaponFireModes { 
    SEMIAUTO,
    FULLAUTO,
    BURST
}

public struct WeaponFireMode {
    public WeaponFireModes mode;
    public int penalty;
}

public enum WeaponTrait { 
}
[SerializeField, Serializable]
public class Weapon {
    public enum WEAPON_ID 
    { 
        HUNTING_RIFLE,
        VARMINT_RIFLE,
        BASIC_PISTOL,
        DEBUG_NONE,
        DEBUG_BLASTER,
        M1911,
        GARAND,
        LASER_PISTOL,
        GHOUL_HANDS,
    }

    public string Name { get; set; }
    public string ID { get; set; }
    public DamageType DamageType { get; set; }
    public WeaponType WeaponType { get; set; }
    // Why is this seperate? Certain weapons are of a unique type and may have a different skill required for operation.
    public GameCharacter.Skill WeaponSkill { get; set; }
    public DicePool DamageDice { get; set; }
    public int Range { get; set; }
    public string Ammo { get; set; }
    public int MagAmmo { get; set; }
    public int MagCapacity { get; set; }
    public WeaponTrait Traits { get; set; }

    public Weapon(string name, string id, DamageType damageType, WeaponType weaponType, GameCharacter.Skill skill,
                  DicePool dicePool, int range, int capacity, string ammo = null,  WeaponTrait traits = 0) {
        Name = name;
        ID = id;
        this.DamageType = damageType;
        this.WeaponType = weaponType;
        WeaponSkill = skill;
        DamageDice = dicePool;
        Range = range;
        MagAmmo = capacity;
        Ammo = ammo;
        MagCapacity = capacity;
        Traits = traits;
    }
}

public static class WeaponFactory {

    private static Dictionary<Weapon.WEAPON_ID, System.Func<Weapon>> weaponMap = new Dictionary<Weapon.WEAPON_ID, System.Func<Weapon>>
    {
        [Weapon.WEAPON_ID.DEBUG_NONE] = BuildVoid,
        [Weapon.WEAPON_ID.HUNTING_RIFLE] = BuildHuntingRifle,
        [Weapon.WEAPON_ID.BASIC_PISTOL] = BuildBasicPistol,
        [Weapon.WEAPON_ID.DEBUG_BLASTER] = BuildGodGun,
        [Weapon.WEAPON_ID.M1911] = Build45Pistol,
        [Weapon.WEAPON_ID.GARAND] = BuildGarand,
        [Weapon.WEAPON_ID.VARMINT_RIFLE] = BuildVarmintRifle,
        [Weapon.WEAPON_ID.GHOUL_HANDS] = BuildGhoulHands,

    };

    private static Weapon BuildGhoulHands()
    {
        DicePool pool = new DicePool();
        pool.Dice.Add(Die.makeDie(6));

        return new Weapon("Ghoul Hands", "ghoulhands", DamageType.KINETIC, WeaponType.UNARMED, GameCharacter.Skill.UNARMED, pool, 1, -1);
    }

    private static Weapon BuildVarmintRifle()
    {
        DicePool pool = new DicePool();
        pool.Dice.Add(Die.makeDie(8));

        return new Weapon("Varmint Rifle", "varmintrifle", DamageType.KINETIC, WeaponType.BALLISTIC, GameCharacter.Skill.BALLISTIC, pool, 8, 5, "556");
    }
    // TODO CHANGE AMMO ID
    private static Weapon BuildLaserPistol()
    {
        DicePool pool = new DicePool();
        pool.Dice.Add(Die.makeDie(6));
        pool.Offset = 2;

        return new Weapon("Laser Pistol", "laserpistol", DamageType.ENERGY, WeaponType.ENERGY, GameCharacter.Skill.ENERGY, pool, 6, 20, "9mm");
    }

    public static Weapon Build(Weapon.WEAPON_ID wid)
    {
        Weapon weapon = weaponMap[wid]();

        return weapon;
    }

    public static Weapon BuildBasicPistol()
    {
        DicePool pool = new DicePool();
        pool.Dice.Add(Die.makeDie(6));
        pool.Offset = 1;

        return new Weapon("Basic Pistol", "basicpistol", DamageType.KINETIC, WeaponType.BALLISTIC, GameCharacter.Skill.BALLISTIC, pool, 4, 15, "9mm");
    }
    private static Weapon BuildGodGun()
    {
        DicePool dice = new DicePool();
        dice.Offset = 100;
        return new Weapon("GODGUN", "debuggodgun", DamageType.DIRECT, WeaponType.NONE, GameCharacter.Skill.BALLISTIC, dice, 999, -1);
    }

    private static Weapon BuildVoid()
    {
        return new Weapon("", "debugnull", DamageType.DIRECT, WeaponType.NONE, GameCharacter.Skill.BALLISTIC, new DicePool(), 0, -1);
    }
    // TODO CHANGE AMMO ID
    private static Weapon BuildGarand()
    {
        DicePool pool = new DicePool();
        pool.Dice.Add(Die.makeDie(6));
        pool.Dice.Add(Die.makeDie(6));
        pool.Offset = 6;

        return new Weapon("M1 Garand", "m1garand", DamageType.KINETIC, WeaponType.NONE, GameCharacter.Skill.BALLISTIC, pool, 7, 8, "9mm");
    }
    // TODO CHANGE AMMO ID
    private static Weapon Build45Pistol()
    {
        DicePool dice = new DicePool();
        dice.Dice.Add(Die.makeDie(8));
        dice.Offset = 4;

        return new Weapon("45Pistol", "m1911", DamageType.KINETIC, WeaponType.BALLISTIC, GameCharacter.Skill.BALLISTIC, dice, 5, 7, "9mm");
    }
    // TODO CHANGE AMMO ID
    public static Weapon BuildHuntingRifle(){
        DicePool dice = new DicePool();
        // 2d10 damage pool
        dice.Dice.Add(Die.makeDie(6));
        dice.Dice.Add(Die.makeDie(6));
        dice.Offset = 6;
        // Damage range 8 - 18

        return new Weapon("Hunting Rifle", "huntingrifle", DamageType.KINETIC, WeaponType.BALLISTIC, GameCharacter.Skill.BALLISTIC, dice, 9, 5, "9mm");
    }
}

