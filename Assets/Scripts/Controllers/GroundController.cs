using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct PathNode
{
    public Vector3Int Position { get; private set; }
    public int Cost { get; private set; }

    public PathNode(Vector3Int v, int c) { Cost = c; Position = v; }
}

public struct PathInfo
{
    public List<PathNode> Nodes;
    public int Cost 
    { 
        get 
        {
            int sum = 0;
            foreach (PathNode n in Nodes)
                sum += n.Cost;
            return sum;
        } 
    }

    public PathNode LastNode { get => Nodes[Nodes.Count-1];}

    public PathInfo(List<PathNode> n)
    {
        Nodes = n;
    }
}

[Flags]
public enum TerrainFlags
{
    NORMAL = 0,
    ROUGH = 1 << 0,
    IMPASSIBLE = 1 << 1,
    LIGHT_COVER = 1 << 2,
    MEDIUM_COVER = 1 << 3,
    FULL_COVER = 1 << 4,
    LIGHT_OBSCUREMENT = 1 << 5,
    FULL_OBSCUREMENT = 1 << 6,
    BLOCKS_LOS = 1 << 7
}

public class GroundController : MonoBehaviour
{
    private List<Vector3Int> HEX_NEIGHBORS { get; set; } = new List<Vector3Int>
    {
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(1, 1, 0)
    };

    public Dictionary<string, TerrainFlags> TerrainInfo { get; private set; } = new Dictionary<string, TerrainFlags>
    {
        ["sand-hex"] = TerrainFlags.NORMAL,
        ["sand_hex"] = TerrainFlags.NORMAL,
        ["hex_base"] = TerrainFlags.IMPASSIBLE,
        ["full-cover-hex"] = TerrainFlags.FULL_COVER | TerrainFlags.IMPASSIBLE | TerrainFlags.BLOCKS_LOS,
        ["full-cover"] = TerrainFlags.FULL_COVER | TerrainFlags.BLOCKS_LOS,
        ["medium-cover-hex"] = TerrainFlags.MEDIUM_COVER | TerrainFlags.IMPASSIBLE,
        ["medium-cover"] = TerrainFlags.MEDIUM_COVER | TerrainFlags.ROUGH,
        ["light-cover-hex"] = TerrainFlags.LIGHT_COVER | TerrainFlags.IMPASSIBLE,
        ["light-cover"] = TerrainFlags.LIGHT_COVER | TerrainFlags.ROUGH,
        ["full-obscure"] = TerrainFlags.FULL_OBSCUREMENT,
        ["light-obscure"] = TerrainFlags.LIGHT_OBSCUREMENT
    };

    public Dictionary<string, int> CoverBonus { get; private set; } = new Dictionary<string, int>
    {
        ["full-cover-hex"] = 6,
        ["full-cover"] = 6,
        ["medium-cover-hex"] = 4,
        ["medium-cover"] = 4,
        ["light-cover-hex"] = 2,
        ["light-cover"] = 2
    };

    public Grid ControlledGrid { get => GetComponent<Grid>();}
    internal int GetCoverBonus(Vector3Int tilePos)
    {
        foreach (Tilemap tilemap in GetComponentsInChildren<Tilemap>())
        {
            if (tilemap.name != "Cover" || tilemap.GetTile(tilePos) == null)
                continue;

            string tileName = tilemap.GetTile(tilePos).name;
            if (CoverBonus.ContainsKey(tileName))
                return CoverBonus[tileName];
        }
        return 0;
    }

    public bool HasLineOfSight(Vector3Int startTile, Vector3Int goalTile)
    {
        return HasLineOfSight(ControlledGrid.GetCellCenterWorld(startTile), ControlledGrid.GetCellCenterWorld(goalTile));
    }


    public bool HasLineOfSight(Creature creature, Creature target) {
        return HasLineOfSight(creature.transform.position, target.transform.position);
    }
    public bool HasLineOfSight(Vector3 origin, Vector3 goal) {
        Vector3 direction = (goal-origin);
        direction /= direction.magnitude;

        TerrainFlags flags;

        RaycastHit2D[] results;

        results = Physics2D.RaycastAll(origin, direction, Vector3.Distance(origin, goal), 1 << LayerMask.NameToLayer("Cover"));

        foreach (RaycastHit2D raycast in results) {
            if (raycast && raycast.collider.TryGetComponent(out Tilemap tilemap)) {
                Vector3Int cell = tilemap.layoutGrid.WorldToCell(raycast.point + (Vector2) (direction * .1f)); // This will force it slightly into the hex that it hit.
                flags = GetTileFlags(cell);
                if (TileHasFlag(cell, TerrainFlags.BLOCKS_LOS))
                {
                    return false;
                }
            }
        }

        return true;
    }
    /*
     * Both start and goal must be hex positions; not actual world points.
     */
    public PathInfo GetPath(Vector3Int start, Vector3Int goal) {
        /**
         * We are going to do some a-star shit baby!
         */
        List<PathNode> path = new List<PathNode>();

        Dictionary<Vector3Int, float> unexplored = new Dictionary<Vector3Int, float>();

        float bestValue = float.MaxValue;
        Vector3Int bestNode = start;

        Dictionary<Vector3Int, int> costDictionary = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        Vector3Int currentCell = new Vector3Int();
        bool goalFound = false;
        
        unexplored[start] = bestValue;

        bool neighborCostKnown;
        costDictionary.Add(start, 0);


        // If we can't get to that tile in the first place, so no reason to waste time.
        // If there is already something there, we can't do anything about it.
        Vector3 realGoal = ControlledGrid.CellToWorld(goal);
        if (!(TileHasFlag(goal, TerrainFlags.IMPASSIBLE) || Physics2D.OverlapPoint(new Vector2(realGoal.x, realGoal.y), 1 << LayerMask.NameToLayer("Beings")) != null))
        {
            while (unexplored.Count > 0)
            {
                bestValue = float.MaxValue;
                foreach (Vector3Int unexploredTile in unexplored.Keys)
                {
                    if (unexplored[unexploredTile] < bestValue)
                    {
                        bestValue = unexplored[unexploredTile];
                        bestNode = unexploredTile;
                    }
                }

                currentCell = bestNode;
                unexplored.Remove(bestNode);

                if (currentCell == goal)
                {
                    goalFound = true;
                    break;
                }

                foreach (Vector3Int neighborCell in GetTiles(currentCell, 1))
                {
                    if (TileHasFlag(neighborCell, TerrainFlags.IMPASSIBLE))
                        continue;

                    costDictionary.TryGetValue(currentCell, out int tileCost);
                    tileCost += GetTileCost(neighborCell);
                    neighborCostKnown = costDictionary.TryGetValue(neighborCell, out int neighborCost);

                    if (!neighborCostKnown || tileCost < neighborCost)
                    {

                        cameFrom[neighborCell] = currentCell;
                        costDictionary[neighborCell] = tileCost;
                        bool costKnown = unexplored.TryGetValue(neighborCell, out float neighborWeight);
                        if (!costKnown || neighborWeight < tileCost + Vector3Int.Distance(neighborCell, goal))
                        {
                            unexplored[neighborCell] = tileCost + Vector3Int.Distance(neighborCell, goal);
                        }
                    }
                }
            }
        }


        if (goalFound)
        {
            while (cameFrom.ContainsKey(currentCell))
            {

                path.Add(new PathNode(currentCell, costDictionary[currentCell] - costDictionary[cameFrom[currentCell]]));
                currentCell = cameFrom[currentCell];

            }
            path.Add(new PathNode(start, 0));
            path.Reverse();

            return new PathInfo(path);
        }

        return new PathInfo(path);
    }

    public List<Vector3Int> GetTiles(Vector3Int tile, int radius) 
    {
        List<Vector3Int> neighbors = new List<Vector3Int> { tile };

        if (radius != 0)
        {
            foreach (Vector3Int direction in HEX_NEIGHBORS)
            {
                // So, because of the offset, hex coordinate are dependent on if row is even.
                Vector3Int adjDir = new Vector3Int(direction.x, direction.y, 0);
                if (tile.y % 2 == 0 && direction.y != 0)
                {
                    adjDir.x -= 1;
                }

                neighbors.Add(tile + adjDir);
                foreach (Vector3Int neighbor in GetTiles(tile + adjDir, radius - 1))
                {
                    if (!neighbors.Contains(neighbor))
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    public string GetTileNames(Vector3Int tile)
    {
        string name = "";
        foreach (Tilemap tilemap in ControlledGrid.GetComponentsInChildren<Tilemap>())
        {
            if (tilemap.GetTile(tile) == null)
            {
                continue;
            }
            name = $"{name}{tilemap.GetTile(tile).name},";
        }

        return name;
    }

    public TerrainFlags GetTileFlags(Vector3Int tile)
    {
        TerrainFlags flags = TerrainFlags.NORMAL;
        int onMaps = 0;
        foreach (Tilemap tilemap in ControlledGrid.GetComponentsInChildren<Tilemap>())
        {
            if (tilemap.GetTile(tile) == null)
            {
                continue;
            }
            string name = tilemap.GetTile(tile).name;
            TerrainInfo.TryGetValue(name, out TerrainFlags tileFlags);
            onMaps += 1;
            flags |= tileFlags;
        }

        if (onMaps == 0)
        {
            flags |= TerrainFlags.IMPASSIBLE;
        }

        return flags;
    }

    public bool TileHasFlag(Vector3Int tile, TerrainFlags flag)
    {
        if (flag == TerrainFlags.NORMAL)
            return GetTileFlags(tile) == TerrainFlags.NORMAL;
        return (GetTileFlags(tile) & flag) == flag;
    }

    public int GetTileCost(Vector3Int tile) 
    {
        TerrainFlags flags = GetTileFlags(tile);
        int tileCost;
        int personCost = IsTileOccupied(tile) ? 1 : 0;

        if ((flags & TerrainFlags.IMPASSIBLE) == TerrainFlags.IMPASSIBLE)
        {
            tileCost = 99999;
        }
        else if ((flags & TerrainFlags.ROUGH) == TerrainFlags.ROUGH)
        {
            tileCost = 2;
        }
        else
        {
            tileCost = 1;
        }
        return tileCost + personCost;
    }

    public bool CanMoveToTile(Vector3Int tile) {
        return !(IsTileOccupied(tile) || TileHasFlag(tile, TerrainFlags.FULL_COVER | TerrainFlags.IMPASSIBLE));
    }

    public bool IsTileOccupied(Vector3Int tile)
    {
        Collider2D col = Physics2D.OverlapPoint(ControlledGrid.CellToWorld(tile));
        return col?.GetComponent<Creature>() != null;
    }
}