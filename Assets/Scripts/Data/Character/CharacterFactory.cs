using System;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterFactory
{
    private static Dictionary<NPCDefintions.NPC_ID, System.Func<GameCharacter>> builderMap = new Dictionary<NPCDefintions.NPC_ID, System.Func<GameCharacter>>
    {
        [NPCDefintions.NPC_ID.BANDIT] = BuildGenericRaider,
        [NPCDefintions.NPC_ID.GHOUL] = BuildFeralGhoul,
        [NPCDefintions.NPC_ID.BIGGHOUL] = BuildGlowingGhoul,
        [NPCDefintions.NPC_ID.CHARLIE] = BuildCharlie,
        [NPCDefintions.NPC_ID.KENZIE] = BuildKenzie,
        [NPCDefintions.NPC_ID.PEYTON] = BuildPeyton,
        [NPCDefintions.NPC_ID.STEVEN] = BuildSteven,
        [NPCDefintions.NPC_ID.MICHAEL] = BuildMichael,
    };

    private static GameCharacter BuildGlowingGhoul()
    {
        GameCharacter gc = new GameCharacter();
        gc.Special = new Dictionary<GameCharacter.SPECIAL, int>
        {
            [GameCharacter.SPECIAL.STRENGTH] = 6,
            [GameCharacter.SPECIAL.PERCEPTION] = 4,
            [GameCharacter.SPECIAL.ENDURANCE] = 7,
            [GameCharacter.SPECIAL.CHARISMA] = 1,
            [GameCharacter.SPECIAL.INTELLIGENCE] = 4,
            [GameCharacter.SPECIAL.AGILITY] = 7,
            [GameCharacter.SPECIAL.LUCK] = 2,
        };

        gc[GameCharacter.Skill.UNARMED] = 4;

        gc.MaxHealth = 28;
        gc.Health = gc.MaxHealth;
        gc.Faction = Faction.FactionInfo.CRITTER;
        gc.Name = "Glowing One";

        return gc;
    }

    private static GameCharacter BuildFeralGhoul()
    {
        GameCharacter gc = new GameCharacter();
        gc.Special = new Dictionary<GameCharacter.SPECIAL, int>
        {
            [GameCharacter.SPECIAL.STRENGTH] = 6,
            [GameCharacter.SPECIAL.PERCEPTION] = 3,
            [GameCharacter.SPECIAL.ENDURANCE] = 6,
            [GameCharacter.SPECIAL.CHARISMA] = 1,
            [GameCharacter.SPECIAL.INTELLIGENCE] = 2,
            [GameCharacter.SPECIAL.AGILITY] = 7,
            [GameCharacter.SPECIAL.LUCK] = 4,
        };

        gc[GameCharacter.Skill.UNARMED] = 2;
        gc.MaxHealth = 10;
        gc.Health = gc.MaxHealth;
        gc.Faction = Faction.FactionInfo.CRITTER;
        gc.Name = "Feral Ghoul";

        return gc;
    }

    private static GameCharacter BuildMichael()
    {
        GameCharacter gc = new GameCharacter();

        gc.Special = new Dictionary<GameCharacter.SPECIAL, int>
        {
            [GameCharacter.SPECIAL.STRENGTH] = 1,
            [GameCharacter.SPECIAL.PERCEPTION] = 8,
            [GameCharacter.SPECIAL.ENDURANCE] = 8,
            [GameCharacter.SPECIAL.CHARISMA] = 1,
            [GameCharacter.SPECIAL.INTELLIGENCE] = 9,
            [GameCharacter.SPECIAL.AGILITY] = 7,
            [GameCharacter.SPECIAL.LUCK] = 6,
        };

        gc[GameCharacter.Skill.ENERGY] = 5;

        gc.MaxHealth = 28;
        gc.Health = gc.MaxHealth;

        gc.Faction = Faction.FactionInfo.PLAYER_PARTY;
        gc.Name = "Jack McKay (Michael)";
        return gc;
    }

    private static GameCharacter BuildCharlie()
    {
        GameCharacter gc = new GameCharacter();

        gc.Special = new Dictionary<GameCharacter.SPECIAL, int>
        {
            [GameCharacter.SPECIAL.STRENGTH] = 5,
            [GameCharacter.SPECIAL.PERCEPTION] = 6,
            [GameCharacter.SPECIAL.ENDURANCE] = 7,
            [GameCharacter.SPECIAL.CHARISMA] = 3,
            [GameCharacter.SPECIAL.INTELLIGENCE] = 8,
            [GameCharacter.SPECIAL.AGILITY] = 8,
            [GameCharacter.SPECIAL.LUCK] = 8,
        };

        gc[GameCharacter.Skill.BALLISTIC] = 5;

        gc.MaxHealth = 26;
        gc.Health = gc.MaxHealth;

        gc.Faction = Faction.FactionInfo.PLAYER_PARTY;
        gc.Name = "Nazem Kadri (Charlie)";
        return gc;
    }

    private static GameCharacter BuildKenzie()
    {
        GameCharacter gc = new GameCharacter();

        gc.Special = new Dictionary<GameCharacter.SPECIAL, int>
        {
            [GameCharacter.SPECIAL.STRENGTH] = 4,
            [GameCharacter.SPECIAL.PERCEPTION] = 7,
            [GameCharacter.SPECIAL.ENDURANCE] = 8,
            [GameCharacter.SPECIAL.CHARISMA] = 3,
            [GameCharacter.SPECIAL.INTELLIGENCE] = 10,
            [GameCharacter.SPECIAL.AGILITY] = 8,
            [GameCharacter.SPECIAL.LUCK] = 3,
        };

        gc[GameCharacter.Skill.ENERGY] = 3;
        gc[GameCharacter.Skill.UNARMED] = 3;

        gc.MaxHealth = 28;
        gc.Health = gc.MaxHealth;

        gc.Faction = Faction.FactionInfo.PLAYER_PARTY;
        gc.Name = "Castor Yarrow (Kenzie)";
        return gc;
    }

    private static GameCharacter BuildPeyton()
    {
        GameCharacter gc = new GameCharacter();

        gc.Special = new Dictionary<GameCharacter.SPECIAL, int>
        {
            [GameCharacter.SPECIAL.STRENGTH] = 3,
            [GameCharacter.SPECIAL.PERCEPTION] = 5,
            [GameCharacter.SPECIAL.ENDURANCE] = 3,
            [GameCharacter.SPECIAL.CHARISMA] = 9,
            [GameCharacter.SPECIAL.INTELLIGENCE] = 6,
            [GameCharacter.SPECIAL.AGILITY] = 8,
            [GameCharacter.SPECIAL.LUCK] = 10,
        };

        gc[GameCharacter.Skill.BALLISTIC] = 2;

        gc.MaxHealth = 18;
        gc.Health = gc.MaxHealth;

        gc.Faction = Faction.FactionInfo.PLAYER_PARTY;
        gc.Name = "Gen. Zandreck (Peyton)";
        return gc;
    }

    private static GameCharacter BuildSteven()
    {
        GameCharacter gc = new GameCharacter();

        gc.Special = new Dictionary<GameCharacter.SPECIAL, int>
        {
            [GameCharacter.SPECIAL.STRENGTH] = 8,
            [GameCharacter.SPECIAL.PERCEPTION] = 6,
            [GameCharacter.SPECIAL.ENDURANCE] = 6,
            [GameCharacter.SPECIAL.CHARISMA] = 4,
            [GameCharacter.SPECIAL.INTELLIGENCE] = 8,
            [GameCharacter.SPECIAL.AGILITY] = 6,
            [GameCharacter.SPECIAL.LUCK] = 2,
        };

        gc[GameCharacter.Skill.BALLISTIC] = 2;
        gc[GameCharacter.Skill.EXPLOSIVES] = 2;

        gc.MaxHealth = 24;
        gc.Health = gc.MaxHealth;

        gc.Faction = Faction.FactionInfo.PLAYER_PARTY;
        gc.Name = "Papermate Classic (Steven)";
        return gc;
    }

    public static GameCharacter Build(NPCDefintions.NPC_ID nid)
    {
        return builderMap[nid]();
    }

    public static GameCharacter BuildGenericRaider()
    {
        GameCharacter gc = new GameCharacter();

        gc.Special = new Dictionary<GameCharacter.SPECIAL, int>
        {
            [GameCharacter.SPECIAL.STRENGTH] = 6,
            [GameCharacter.SPECIAL.PERCEPTION] = 5,
            [GameCharacter.SPECIAL.ENDURANCE] = 6,
            [GameCharacter.SPECIAL.CHARISMA] = 3,
            [GameCharacter.SPECIAL.INTELLIGENCE] = 4,
            [GameCharacter.SPECIAL.AGILITY] = 5,
            [GameCharacter.SPECIAL.LUCK] = 3,
        };

        gc.MaxHealth = 14;
        gc.Health = gc.MaxHealth;

        gc[GameCharacter.Skill.BALLISTIC] = 2;
        
        gc.Faction = Faction.FactionInfo.BANDIT;
        gc.Name = "Raider";
        return gc;
    }
}