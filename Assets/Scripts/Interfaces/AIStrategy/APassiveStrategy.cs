using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APassiveStrategy

{
    // The game object that is using this strategy.
    public GameObject Self { get; set; }
    public abstract void OnInteract();
    public abstract void OnAttacked(DamageDetails dd);
    public abstract void OnIdle();
    public abstract void OnCombatTurn();
}
