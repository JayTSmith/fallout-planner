using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die
{
    public readonly int sides;

    private static Dictionary<int, Die> madeDice = new Dictionary<int, Die>();

    private Die(int s)
    {
        sides = s;
    }

    public static Die makeDie(int s)
    {
        if (!madeDice.ContainsKey(s))
        {
            madeDice[s] = new Die(s);
        }
        return madeDice[s];
    }

    public int Roll(System.Random rand = null)
    {
        if (rand == null)
            return (int)Mathf.Round(Random.value * (sides - 1)) + 1;
        return rand.Next(sides) + 1;
    }

    public static implicit operator int(Die d) => d.Roll();
}

public class DicePool
{
    public List<Die> Dice { get; }
    public int Offset { get; set; }

    public DicePool() {
        Dice = new List<Die>();
    }

    public DicePool(List<Die> d)
    {
        Dice = d;
    }

    public int Roll()
    {
        int sum = 0;
        foreach (Die d in Dice)
        {
            sum += d.Roll();
        }

        return sum + Offset;
    }
}
