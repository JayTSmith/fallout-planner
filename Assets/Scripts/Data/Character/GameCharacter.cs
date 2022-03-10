using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter
{
    public enum SPECIAL { 
        STRENGTH,
        PERCEPTION,
        ENDURANCE,
        CHARISMA,
        INTELLIGENCE,
        AGILITY,
        LUCK
    }

    public enum Skill { 
        BALLISTIC,
        ENERGY,
        OUTDOORSMAN,
        EXPLOSIVES,
        MELEE,
        UNARMED
    }

    public static Dictionary<Skill, SPECIAL> SkillSpecial = new Dictionary<Skill, SPECIAL> {
        [Skill.BALLISTIC] = SPECIAL.AGILITY,
        [Skill.ENERGY] = SPECIAL.INTELLIGENCE,
        [Skill.OUTDOORSMAN] = SPECIAL.ENDURANCE,
        [Skill.EXPLOSIVES] = SPECIAL.PERCEPTION,
        [Skill.MELEE] = SPECIAL.STRENGTH,
        [Skill.UNARMED] = SPECIAL.ENDURANCE
    };

    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public Dictionary<SPECIAL, int> Special { get; set; }
    public Dictionary<Skill, int> Skills { get; set; }

    public int this[SPECIAL s] {
        get => Special[s];
        set => Special[s] = Mathf.Min(10, Mathf.Max(value, 1));
    }
    public int this[Skill s] { 
        get => Skills[s];
        set => Skills[s] = Mathf.Min(10, Mathf.Max(value, 0));
    }

    // Combat Information
    public Faction.FactionInfo Faction { get; set; }
    public int Initiative { get; private set; }
    public int CurrentMovePoints { get; set; }
    public int MaxMovePoints { get => 4 + GetSpecialMod(SPECIAL.AGILITY); }
    public int AC {
        get
        {
            if (EquippedArmor == null)
            {
                return 8 + GetSpecialMod(SPECIAL.AGILITY);
            }
            else
            {
                return EquippedArmor.AC;
            }
        }
    }
    public Weapon EquippedWeapon { get; set; }

    public Armor EquippedArmor { get; set; }
    public string Name { get; internal set; } = "";

    public GameCharacter() 
    {
        Special = new Dictionary<SPECIAL, int>();
        Skills = new Dictionary<Skill, int>();

        foreach (SPECIAL special in System.Enum.GetValues(typeof(SPECIAL)))
        {
            this[special] = 1;
        }

        foreach (Skill skill in System.Enum.GetValues(typeof(Skill)))
        {
            this[skill] = 0;
        }

        MaxHealth = 1;
        Health = 1;
        CurrentMovePoints = 1;
        EquippedWeapon = WeaponFactory.BuildHuntingRifle();
    }

    internal int GetDT(DamageType damageType)
    {
        if (EquippedArmor == null) 
            return 0;

        switch (damageType)
        {
            case DamageType.KINETIC:
                return EquippedArmor.BDT;
            case DamageType.ENERGY:
                return EquippedArmor.EDT;
            case DamageType.EXPLOSIVE:
                return EquippedArmor.XDT;
            default:
                return 0;
        }
    }

    public bool RollCritSuccess()
    {
        return Die.makeDie(20).Roll() > 18 - GetSpecialMod(SPECIAL.LUCK);
    }

    public bool RollCritFailure() 
    {
        return Die.makeDie(20).Roll() < 4 - GetSpecialMod(SPECIAL.LUCK);
    }

    public int RollInitiative()
    {
        Initiative = Mathf.RoundToInt(Random.value * 20) + GetSpecialMod(SPECIAL.AGILITY);
        return Initiative;
    }

    internal int GetBonusDamage()
    {
        if ((EquippedWeapon.WeaponType & (WeaponType.MELEE | WeaponType.UNARMED)) != 0)
        {
            if (EquippedWeapon.WeaponType == WeaponType.MELEE)
                return GetSpecialMod(SPECIAL.STRENGTH);
            if (EquippedWeapon.WeaponType == WeaponType.UNARMED)
                return GetSpecialMod(SPECIAL.ENDURANCE);
        }
        return 0;
    }

    public int GetSkillMod(Skill s)
    {
        return this[s] + GetSpecialMod(SkillSpecial[s]);
    }
    public int GetSpecialMod(SPECIAL s)
    {
        return (this[s] - 5) / 2;
    }
    internal int GetSkillSpecialMod(Skill s)
    {
        return GetSpecialMod(SkillSpecial[s]);
    }
}
