using System;
using System.Collections;
using System.Collections.Generic;
using TTRPGSimulator.Combat.Simulation;
using UnityEngine;
using UnityEngine.UIElements;

namespace TTRPGSimulator.Controller
{
    [RequireComponent(typeof(InfoTracker)), RequireComponent(typeof(UIDocument))]
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        VisualElement CombatListItemTemplate;

        Label SimCountLabel;
        Label SimChanceLabel;
        ScrollView SimEventsScrollView;
        ScrollView SimScrollView;

        Dictionary<string, IList<ASimulationEvent>> Simulations = new Dictionary<string, IList<ASimulationEvent>>();

        // Start is called before the first frame update
        void Awake()
        {
            UIDocument doc = GetComponent<UIDocument>();
            SimChanceLabel = doc.rootVisualElement.Q<Label>("SimChanceDataLbl");
            SimCountLabel = doc.rootVisualElement.Q<Label>("SimNumberDataLbl");


            SimEventsScrollView = doc.rootVisualElement.Q<ScrollView>("SimEventsScrollView");
            SimScrollView = doc.rootVisualElement.Q<ScrollView>("SimScrollList");
        }

        public void AddSimulation(string name, IList<ASimulationEvent> events) {
            Label lbl = new Label(name);
            lbl.RegisterCallback<MouseDownEvent, string>(DisplaySimulation, name);
            SimScrollView.Add(lbl);

            Simulations.Add(name, events);
        }

        private void DisplaySimulation(MouseDownEvent evt, string name) {
            SimEventsScrollView.Clear();

            foreach (ASimulationEvent simEvent in Simulations[name]) 
            {
                Debug.LogWarning("Adding event");
                Label lbl = new Label(simEvent.GetText()) { style = { whiteSpace = WhiteSpace.Normal} };
                SimEventsScrollView.Add(lbl);
            }
        }

        public void SetSimulationCount(int count) 
        {
            SimCountLabel.text = count.ToString();
        }

        public void SetSimulationChance(double chance)
        {
            SimChanceLabel.text = chance.ToString("P");
        }
    }
}