using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    bool isPlayer;

    [SerializeField]
    private GroundController groundController;
    private Grid gameGrid;

    public GameCharacter GameCharacter { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        GameCharacter = CharacterFactory.BuildHumanRandom();
        GameCharacter.Health = GameCharacter.MaxHealth();

        GetComponent<Creature>().GameCharacter = GameCharacter;

        gameGrid = groundController.ControlledGrid;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPos.z = 0;  // This doesn't matter to us. It's camera distance which is useless for this and messes with the return results.
            Vector3Int cell = gameGrid.WorldToCell(clickPos);

            GetComponent<Creature>().MoveToCell(cell);
        }

        if (Input.GetMouseButtonDown(1))
        {

            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPos.z = 0;  // This doesn't matter to us. It's camera distance which is useless for this and messes with the return results.
            Vector3Int cell = gameGrid.WorldToCell(clickPos);
        }
    }
    public void TakeDamage(int damage)
    {
        GameCharacter.Health -= damage;
        if (GameCharacter.Health <= 0) 
        {
            Debug.LogWarning("Player has DIED!");
        }
    }
}
