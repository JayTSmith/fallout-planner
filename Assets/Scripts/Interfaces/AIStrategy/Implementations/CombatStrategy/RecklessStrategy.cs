using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TTRPGSimulator.Combat.Simulation;
using UnityEngine;

namespace TTRPGSimulator.AI
{
    public class RecklessStrategy : ACombatStrategy
    {
        private bool moveUsed = false;
        private bool combatUsed = false;
        private bool supportUsed = false;

        public RecklessStrategy(GameObject go)
        {
            Self = go;
            IsBusy = false;
        }

        public override void DoCombatAction()
        {
            Creature closestEnemy = GetClosestEnemy();
            bool isWeaponMelee = Creature.GameCharacter.EquippedWeapon.WeaponType == WeaponType.UNARMED || Creature.GameCharacter.EquippedWeapon.WeaponType == WeaponType.MELEE;
            if ((Creature.GameCharacter.EquippedWeapon.MagAmmo > 0 || isWeaponMelee)
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
                combatUsed = true;
            }

        }

        private Creature GetClosestEnemy()
        {
            Creature closest = null;
            float distance = float.MaxValue;

            foreach (Creature combatant in BattleController.Entities)
            {
                if (combatant == null) continue;
                if (!combatant.isActiveAndEnabled) continue;
                if (combatant.Equals(Self) || combatant.Equals(closest))
                {
                    continue;
                }

                if ((combatant.GameCharacter.Faction & Creature.GameCharacter.Faction) == Faction.FactionInfo.NONE)
                {
                    float curDist = Vector3Int.Distance(combatant.CurrentCell, Creature.CurrentCell);
                    // If we can't get to the creature, then it doens't matter if they're closer.
                    if (curDist < distance)
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
            IsBusy = true;

            if (closestEnemy == null)
            {
                IsBusy = false;
                yield break;
            }

            Vector3Int goalTile = Creature.CurrentCell;
            float goalDistance = Vector3Int.Distance(closestEnemy.CurrentCell, goalTile);
            if (goalDistance < 2) 
            {
                IsBusy = false;
                yield break;
            }

            foreach (Vector3Int tile in GroundController.GetTiles(closestEnemy.CurrentCell, 4))
            {
                if (Self == null) yield break;

                if (GroundController.CanMoveToTile(tile)
                    && (Creature.GameCharacter.EquippedWeapon.WeaponType == WeaponType.UNARMED || Creature.GameCharacter.EquippedWeapon.WeaponType == WeaponType.MELEE || GroundController.HasLineOfSight(tile, closestEnemy.CurrentCell)))
                {
                    float tileDistance = Vector3Int.Distance(closestEnemy.CurrentCell, tile);
                    if (tileDistance < goalDistance)
                    {
                        goalDistance = Vector3Int.Distance(closestEnemy.CurrentCell, goalTile);
                        goalTile = tile;
                    }
                }
                yield return null;
            }

            PathInfo path = GroundController.GetPath(Creature.CurrentCell, goalTile);
            path = GroundController.FixPath(path, Creature.GameCharacter.CurrentMovePoints);

            if (path.Nodes.Count <= 1)
            {
                // Means either we can't get to them or we're already hugging them.
                IsBusy = false;
                yield break;
            }
            Creature.GameCharacter.CurrentMovePoints -= path.Cost;

            // For some reason, the path we made is invalid. Cancel early.
            if (!Creature.MoveToCell(path))
                yield break;

            IsBusy = false;
            // Since this could be destroyed in the middle of movement, we must do a null check.
            yield return new WaitWhile(() => Self != null && Creature.IsMoving);

        }
    }
}