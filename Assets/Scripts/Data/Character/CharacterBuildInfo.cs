using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CharacterBuildInfo 
{
    public Armor.ARMOR_ID armorID;
    public Weapon.WEAPON_ID weaponID;
    public NPCDefintions.NPC_ID npcID;

    public CharacterBuildInfo(Armor.ARMOR_ID aid, Weapon.WEAPON_ID wid, NPCDefintions.NPC_ID nid)
    {
        armorID = aid;
        weaponID = wid;
        npcID = nid;
    }

    public GameCharacter Build()
    {
        GameCharacter gc = CharacterFactory.Build(npcID);
        gc.EquippedWeapon = WeaponFactory.Build(weaponID);
        gc.EquippedArmor = (armorID != Armor.ARMOR_ID.NULL) ? ArmorFactory.Build(armorID) : null;
        return gc;
    }
}