using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TabletopSimulator.Combat;
using UnityEngine;

namespace TabletopSimulator.AI
{
    public class RecklessStrategy : ACombatStrategy
    {
        public RecklessStrategy(GameObject go)
        {
            Self = go;
        }


        public override void DoCombatAction()
        {
            Creature closestEnemy = GetClosestEnemy();
            if (Creature.GameCharacter.EquippedWeapon.MagAmmo > 0
                && closestEnemy != null)
            {
                BattleController.Attack(Creature, closestEnemy);
                //return DeclareAttackEvent();
            }
            else if (Creature.GameCharacter.EquippedWeapon.MagAmmo == 0)
            {
                // creature.GameCharacter.Reload();
            }
            else
            {
                // We ain't got line of sight so oops.
            }

        }

        private Creature GetClosestEnemy()
        {
            Creature closest = null;
            float distance = float.MaxValue;

            foreach (Creature combatant in BattleController.Entities)
            {
                if (combatant.Equals(Self) || combatant.Equals(closest))
                {
                    continue;
                }

                if ((combatant.GameCharacter.Faction & Creature.GameCharacter.Faction) == Faction.FactionInfo.NONE)
                {
                    float curDist = Vector3Int.Distance(combatant.CurrentCell, Creature.CurrentCell);
                    // If we can't get to the creature, then it doens't matter if they're closer.
                    if (curDist < distance && GroundController.GetPath(Creature.CurrentCell, combatant.CurrentCell).Nodes.Count > 0)
                    {
                        closest = combatant;
                        distance = curDist;
                    }
                }
            }

            return closest;
        }

        public override IEnumerator DoMovement()
        {
            //TODO Consider adjusting to move to closest point if impossible to reach enemy.

            // Get as close to the nearest enemy as we can.
            // Stances do not matter rn.
            Creature closestEnemy = GetClosestEnemy();

            if (closestEnemy == null)
            {
                yield break;
            }

            Vector3Int goalTile = Creature.CurrentCell;
            foreach (Vector3Int tile in GroundController.GetTiles(closestEnemy.CurrentCell, 4))
            {
                if (GroundController.CanMoveToTile(tile)
                    && (Creature.GameCharacter.EquippedWeapon.WeaponType == WeaponType.UNARMED || Creature.GameCharacter.EquippedWeapon.WeaponType == WeaponType.MELEE || GroundController.HasLineOfSight(tile, closestEnemy.CurrentCell)))
                {
                    float goalDistance = Vector3Int.Distance(closestEnemy.CurrentCell, goalTile);
                    float tileDistance = Vector3Int.Distance(closestEnemy.CurrentCell, tile);
                    if (tileDistance < goalDistance)
                    {
                        goalTile = tile;
                    }
                }
            }

            PathInfo path = GroundController.GetPath(Creature.CurrentCell, goalTile);
            while (path.Cost > Creature.GameCharacter.CurrentMovePoints)
            {
                path.Nodes.RemoveAt(path.Nodes.Count - 1);
            }

            if (path.Nodes.Count <= 1)
            {
                // Means either we can't get to them or we're already hugging them.
                yield break;
            }

            //path.Nodes.RemoveAt(path.Nodes.Count); // Don't wanna move on top of the enemy.
            Creature.GameCharacter.CurrentMovePoints -= path.Cost;

            Creature.MoveToCell(path.LastNode.Position);
            // Since this could be destroyed in the middle of movement, we must do a null check.
            yield return new WaitWhile(() => Self != null && Creature.IsMoving);

        }
    }
}