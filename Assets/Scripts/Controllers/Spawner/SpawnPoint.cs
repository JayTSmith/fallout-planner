using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GroundController GroundController;
    public BattleController BattleController;

    [SerializeField]
    CharacterBuildInfo spawn;
    [Range(1, 5)]
    public int spawnRadius = 1;
    [Range(1, 10)]
    public int spawnNumber = 1;

    public void DespawnAll()
    {
        GameObject go;
        for (int i = 0; i < transform.childCount; i++)
        {
            go = transform.GetChild(i).gameObject;
            if (go.GetComponent<Creature>() != null)
            {
                go.SetActive(false);
                Destroy(go);
            }
        }
    }
    private List<Vector3Int> GetSpawnTiles()
    {
        List<Vector3Int> validSpawnTiles = new List<Vector3Int>();
        SpawnPoint[] pts = GetComponentsInChildren<SpawnPoint>();

        foreach (SpawnPoint point in pts)
        {
            foreach (Vector3Int tile in GroundController.GetTiles(GroundController.ControlledGrid.WorldToCell(point.transform.position), spawnRadius))
            {
                if (GroundController.CanMoveToTile(tile))
                {
                    validSpawnTiles.Add(tile);
                }
            }
        }

        return validSpawnTiles;
    }

    private void SpawnEntity(Vector3Int tile)
    {
        GameCharacter spawnedCharacter = spawn.Build();
        GameObject body = Instantiate(Resources.Load("Prefabs/BasicEnemy") as GameObject, GroundController.ControlledGrid.GetCellCenterWorld(tile), Quaternion.identity, transform);

        body.name = spawnedCharacter.Name;

        body.GetComponent<Creature>().GameCharacter = spawnedCharacter;
        body.GetComponent<Creature>().GroundController = GroundController;
        body.GetComponent<AIController>().BattleController = BattleController;
        body.GetComponent<AIController>().GroundController = GroundController;

        body.GetComponentInChildren<SpriteRenderer>().color = NPCDefintions.NPCColors[spawn.npcID];
    }

    public void Spawn(bool despawn = false)
    {
        if (despawn)
            DespawnAll();

        List<Vector3Int> validSpawnTiles = GetSpawnTiles();
        int validSpawnNumber = spawnNumber;

        if (validSpawnNumber > validSpawnTiles.Count)
        {
            Debug.LogWarning($"Unable to spawn {spawnNumber} entities in a {spawnRadius}-hex radius! (Total hexes: {validSpawnTiles.Count})", gameObject);
        }

        while (validSpawnNumber > 0)
        {
            int spawnTile = Random.Range(0, validSpawnTiles.Count);
            SpawnEntity(validSpawnTiles[spawnTile]);
            validSpawnTiles.RemoveAt(spawnTile);
            validSpawnNumber--;
        }
    }

    public void Start()
    {
        Spawn();
    }
}
