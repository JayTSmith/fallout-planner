using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public GroundController GroundController;
    [SerializeField]
    [Range(0.0f, 1f)]
    float baseMoveTime = .03f;
    [SerializeField]
    bool scaleMoveTime = false;

    public Vector3Int CurrentCell { get => GroundController.ControlledGrid.WorldToCell(transform.position); }
    public bool IsMoving { get => curPath != null; }

    public GameCharacter GameCharacter { get; set; }

    private Grid GameGrid { get => GroundController.ControlledGrid; }

    private Vector3 moveSpeed;
    private float moveTime;
    private List<Vector3Int> curPath = null;
    private int curNode;

    // Update is called once per frame
    void Update()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (curPath?.Count > 0)
        {
            Vector3 goal = GameGrid.CellToWorld(curPath[curNode]);
            if (transform.position != goal)
            {
                transform.position = Vector3.SmoothDamp(transform.position, goal, ref moveSpeed, moveTime);
            }
            else if (curNode + 1 < curPath.Count)
            {
                curNode++;
            }
            else
            {
                curPath = null;
            }
        }

        if (collider && Input.GetMouseButtonDown(0) && collider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            Debug.Log($"{GameCharacter.Name} - Health: {GameCharacter.Health} / {GameCharacter.MaxHealth}");
        }
    }

    public int CoverBonus(Vector3 from)
    {
        // Check adjacent tile from the direction of the attack for a cover bonus
        return GroundController.GetCoverBonus(GroundController.ControlledGrid.WorldToCell(Vector3.MoveTowards(transform.position, from, 1.0f)));
    }

    public bool MoveToCell(PathInfo path) {

        List<Vector3Int> nodes = new();

        foreach (PathNode pathNode in path.Nodes)
        {
            nodes.Add(pathNode.Position);
        }

        curPath = nodes;
        curNode = 0;

        if (curPath.Count == 0)
        {
            curPath = null;
            return false;
        }

        return true;
    }

    public bool MoveToCell(Vector3Int cell)
    {
        if (GroundController.CanMoveToTile(cell) && cell != CurrentCell)
        {
            // Debug.Log(string.Format("Making Path to {0} from {1}",cell, CurrentCell), this.gameObject);
            PathInfo path;
            List<Vector3Int> nodes = new();

            path = GroundController.GetPath(GameGrid.WorldToCell(transform.position), cell);
            foreach (PathNode pathNode in path.Nodes)
                nodes.Add(pathNode.Position);

            curPath = nodes;
            curNode = 0;

            if (curPath.Count == 0)
            {
                curPath = null;
                return false;
            }

            moveTime = baseMoveTime;
            if (scaleMoveTime)
            {
                moveTime = baseMoveTime / curPath.Count;
            }

            Debug.DrawLine(transform.position, GameGrid.GetCellCenterWorld(curPath[0]), Color.green, 3.0f);
            for (int i = 0; i < curPath.Count - 1; i++)
            {
                Debug.DrawLine(GameGrid.GetCellCenterWorld(curPath[i]), GameGrid.GetCellCenterWorld(curPath[i + 1]), Color.green, 3.0f);
            }

            return true;
        }
        return false;
    }

    public bool TakeDamage(int damage) 
    {
        GameCharacter.Health -= damage;
        if (GameCharacter.Health <= 0)
        {
            gameObject.SetActive(false);
        }

        return GameCharacter.Health <= 0;
    }
}
