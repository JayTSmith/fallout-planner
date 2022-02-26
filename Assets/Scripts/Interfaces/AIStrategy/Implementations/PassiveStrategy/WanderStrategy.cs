using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WanderStrategy : APassiveStrategy
{
    private float timeSinceLastMove = 0f;
    private float moveWait = 3f;

    public WanderStrategy(GameObject go)
    {
        Self = go;
    }

    public override void OnAttacked(DamageDetails dd)
    {
        // TODO Start Combat
    }

    public override void OnIdle()
    {
        Creature c = Self.GetComponent<Creature>();
        if (c)
        {
            if (c.IsMoving)
            {
                timeSinceLastMove = 0.0f;
            }
            else if (timeSinceLastMove >= moveWait)
            {
                int radius = Random.Range(1, 4);
                List<Vector3Int> possibleOptions = c.GroundController.GetTiles(c.CurrentCell, radius);

                int selectedTile;
                do
                {
                    selectedTile = Random.Range(0, possibleOptions.Count);
                } while (!c.GroundController.CanMoveToTile(possibleOptions[selectedTile]));
                c.MoveToCell(possibleOptions[selectedTile]);
            }
            else
            {
                timeSinceLastMove += Time.deltaTime;
            }
        }
    }

    public override void OnInteract()
    {
        // LMAO user cannot interact
    }

    public override void OnCombatTurn()
    {
        throw new System.NotImplementedException();
    }
}
