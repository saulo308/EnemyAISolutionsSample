using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    public class PlannerNode
    {
        public PlannerNode NodeParent;
        public float NodeCost;
        public GoapStateDataDict NodeStates;
        public AGoapAction NodeAction;

        public PlannerNode(PlannerNode parent, float cost, GoapStateDataDict statesData, AGoapAction action)
        {
            NodeParent = parent;
            NodeCost = cost;
            NodeStates = statesData;
            NodeAction = action;
        }
    }

    public class GoapPlanner
    {
        // Public Methods -----------------------------------------------------------------
        public Queue<AGoapAction> CreateNewAgentPlan(List<AGoapAction> agentActionList, GoapStateDataDict agentGoalTargetStates)
        {
            // Check procedural actions

            // Start graph leaves list (we are working it backwards)
            List<PlannerNode> planGraphLeaves = new List<PlannerNode>();
            PlannerNode planGraphRoot = new PlannerNode(null, 0, GoapWorldManager.GoapWorldInstance.GetWorldStates(), null);

            // Create graph with all posible actions to take
            bool bFoundAtLeastOnePath = BuildPlanGraph(planGraphRoot, planGraphLeaves, agentActionList, agentGoalTargetStates);

            // If no plan found, return
            if(!bFoundAtLeastOnePath)
            {
                Debug.LogWarning("No plan found!");
                return null;
            }

            // If found at least one path, get the least expensive one
            PlannerNode cheapestNode = null;
            foreach(var leaf in planGraphLeaves)
            {
                if(cheapestNode == null)
                {
                    cheapestNode = leaf;
                    continue;
                }
                
                if(leaf.NodeCost < cheapestNode.NodeCost)
                    cheapestNode = leaf;
            }

            // Get actions from chosen plan
            List<AGoapAction> resultingActionPlanList = new List<AGoapAction>();
            PlannerNode tmpNode = cheapestNode;
            while(tmpNode != null)
            {
                if(tmpNode.NodeAction != null)
                    resultingActionPlanList.Insert(0, tmpNode.NodeAction);
                tmpNode = tmpNode.NodeParent;
            }

            // Transform list into queue
            Queue<AGoapAction> resultingActionQueue = new Queue<AGoapAction>();
            foreach(var action in resultingActionPlanList)
                resultingActionQueue.Enqueue(action);

           /*  Debug.Log("Plan:");
            foreach(var actionInQueue in resultingActionQueue)
                Debug.Log(actionInQueue.ActionName); */
        
            return resultingActionQueue;
        }

        // Private Methods ---------------------------------------------------------
        bool BuildPlanGraph(PlannerNode parentNode, List<PlannerNode> graphLeaves, List<AGoapAction> availableActions, GoapStateDataDict agentGoalTargetStates)
        {
            bool bFoundAtLeastOnePath = false;

            foreach(var action in availableActions)
            {
                if(action.DoesGivenStateFulfillPreconditions(parentNode.NodeStates))
                {
                    GoapStateDataDict newStateAfterAction = new GoapStateDataDict();
                    newStateAfterAction.MergeWithGoapStateDict(parentNode.NodeStates);
                    newStateAfterAction.MergeWithGoapStateDict(action.ActionEffects);

                    PlannerNode newGraphPlanNode = new PlannerNode(parentNode, parentNode.NodeCost + action.ActionCost, newStateAfterAction, action);
                
                    if(HasGoalBeenAchieved(agentGoalTargetStates, newStateAfterAction))
                    {
                        graphLeaves.Add(newGraphPlanNode);
                        bFoundAtLeastOnePath = true;
                        continue;
                    }

                    List<AGoapAction> newActionSubset = RemoveActionFromList(availableActions, action);
                    bool bFoundPathRecursively = BuildPlanGraph(newGraphPlanNode, graphLeaves, newActionSubset, agentGoalTargetStates);
                    if(bFoundPathRecursively)
                        bFoundAtLeastOnePath = true;
                }
            }

            return bFoundAtLeastOnePath;
        }

        bool HasGoalBeenAchieved(GoapStateDataDict goalTargetStates, GoapStateDataDict statesToCheck)
        {
            foreach(var goalState in goalTargetStates.StateDict)
            {
                if(!statesToCheck.StateDict.ContainsKey(goalState.Key))
                    return false;
            }
            return true;
        }

        List<AGoapAction> RemoveActionFromList(List<AGoapAction> targetActionList, AGoapAction actionToRemove)
        {
            List<AGoapAction> newActionList = new List<AGoapAction>();

            foreach(var action in targetActionList)
            {
                if(!action.Equals(actionToRemove))
                    newActionList.Add(action);
            }

            return newActionList;
        }
    }
}
