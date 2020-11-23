using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GAction : MonoBehaviour {

    // Name of the action
    public string actionName = "Action";
    // Cost of the action
    public float cost = 1.0f;
    // Target where the action is going to take place
    public GameObject target;
    // Store the tag
    public string targetTag;
    // An array of WorldStates of preconditions
    public WorldState[] preConditions;
    // An array of WorldStates of afterEffects
    public WorldState[] afterEffects;
    // Dictionary of preconditions
    public Dictionary<string, int> preconditions;
    // Dictionary of effects
    public Dictionary<string, int> effects;
    // State of the agent
    public WorldStates agentBeliefs;
    // // Access our inventory
    // public GInventory inventory;
    public WorldStates beliefs;
    // Are we currently performing an action?
    public bool running = false;

    // Constructor
    public GAction() {

        // Set up the preconditions and effects
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    private void Awake() {
        // Check if there are any preConditions in the Inspector
        // and add to the dictionary
        if (preConditions != null) {

            foreach (WorldState w in preConditions) {

                // Add each item to our Dictionary
                preconditions.Add(w.key, w.value);
            }
        }

        // Check if there are any afterEffects in the Inspector
        // and add to the dictionary
        if (afterEffects != null) {

            foreach (WorldState w in afterEffects) {

                // Add each item to our Dictionary
                effects.Add(w.key, w.value);
            }
        }
        // Get our agents beliefs
        beliefs = this.GetComponent<GAgent>().beliefs;
    }

    public bool IsAchievable() {

        return true;
    }

    //check if the action is achievable given the condition of the
    //world and trying to match with the actions preconditions
    public bool IsAhievableGiven(Dictionary<string, int> conditions) {

        foreach (KeyValuePair<string, int> p in preconditions) {

            if (!conditions.ContainsKey(p.Key)) {

                return false;
            }
        }
        return true;
    }

    public abstract bool PrePerform();
    public abstract void Perform();
    public abstract bool PostPerform();
}
