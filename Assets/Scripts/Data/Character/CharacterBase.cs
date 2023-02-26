using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterBase
{
    public string Name { get; set; }
    public string ID { get; set; }

    public int MaxHP { get; set; }
    public Dictionary<GameCharacter.SPECIAL, int> Special;
    public Dictionary<GameCharacter.Skill, int> Skills;

    public Vector4 Color { get; set; }
}
