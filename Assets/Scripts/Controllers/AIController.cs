using System.Collections;
using System.Collections.Generic;
using TabletopSimulator.AI;
using UnityEngine;

namespace TabletopSimulator.Controller 
{
    public class AIController : MonoBehaviour
    {
        public ACombatStrategy CombatStrategy { get; set; }

        public GroundController GroundController;
        public BattleController BattleController;
        public Creature Creature { get; set; }
        // Start is called before the first frame update

        void Start()
        {
            Creature = GetComponent<Creature>();


            CombatStrategy = new RecklessStrategy(gameObject);
        }
    }
}