using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public APassiveStrategy PassiveStrategy { get; set; }
    public ACombatStrategy CombatStrategy { get; set; }

    public GroundController GroundController;
    public BattleController BattleController;
    private Grid gameGrid;
    public Creature Creature { get; set; }
    public GameCharacter GameCharacter { get; set; }
    // Start is called before the first frame update

    void Start()
    {
        GameCharacter = CharacterFactory.BuildHumanRandom();
        gameGrid = GroundController.ControlledGrid;

        Creature = GetComponent<Creature>();

        PassiveStrategy = new WanderStrategy(this.gameObject);
        CombatStrategy = new RecklessStrategy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Creature && !BattleController.IsCombat)
        {
            PassiveStrategy.OnIdle();
        }
    }
}
