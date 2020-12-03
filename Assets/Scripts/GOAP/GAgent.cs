﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubGoal {

    // Dictionary to store our goals
    public Dictionary<string, int> sGoals;
    // Bool to store if goal should be removed after it has been achieved
    public bool remove;

    // Constructor
    public SubGoal(string s, int i, bool r) {

        sGoals = new Dictionary<string, int>();
        sGoals.Add(s, i);
        remove = r;
    }
}

public class GAgent : MonoBehaviour {

    // Store our list of actions
    public List<GAction> actions = new List<GAction>();
    // Dictionary of subgoals
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    // // Our inventory
    // public GInventory inventory = new GInventory();
    // Our beliefs
    public WorldStates beliefs = new WorldStates();

    // Access the planner
    GPlanner planner;
    // Action Queue
    Queue<GAction> actionQueue;
    // Our current action
    public GAction currentAction;
    // Our subgoal
    SubGoal currentGoal;

    // Start is called before the first frame update
    public void Start() {

        GAction[] acts = this.GetComponents<GAction>();
        foreach (GAction a in acts) {

            actions.Add(a);
        }
    }

    void LateUpdate() {

        //if there's a current action and it is still running
        if (currentAction != null && currentAction.running) {
            return;
        }

        // Check we have a planner and an actionQueue
        if (planner == null || actionQueue == null) {

            // If planner is null then create a new one
            planner = new GPlanner();

            // Sort the goals in descending order and store them in sortedGoals
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            //look through each goal to find one that has an achievable plan
            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals) {

                actionQueue = planner.plan(actions, sg.Key.sGoals, beliefs);
                // If actionQueue is not = null then we must have a plan
                if (actionQueue != null) {
                    // Set the current goal
                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        // Have we an actionQueue
        if (actionQueue != null && actionQueue.Count == 0) {

            // Check if currentGoal is removable
            if (currentGoal.remove) {

                // Remove it
                goals.Remove(currentGoal);
            }
            // Set planner = null so it will trigger a new one
            planner = null;
        }

        // Do we still have actions
        if (actionQueue != null && actionQueue.Count > 0) {

            // Remove the top action of the queue and put it in currentAction
            currentAction = actionQueue.Dequeue();

            if (currentAction.PrePerform()) {
                currentAction.running = true;
                currentAction.Perform();
            } else {
                // Force a new plan
                actionQueue = null;
            }
        }
    }
}