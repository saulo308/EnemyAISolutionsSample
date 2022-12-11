using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    /**
    * Node that is used to create a tree with multiple paths so we can choose the cheaper path from root to leaf
    * This cheapest path is chosen as the new GoapAgent plan
    */
    public class PlannerNode
    {
        public PlannerNode NodeParent; // Hold reference to parent
        public float NodeCost; // Cost to get to this node, performing actions (this cost is accumulative from previous nodes on tree)
        public GoapStateDataDict NodeStates; // Set of states resulting to get to this node on tree path
        public AGoapAction NodeAction; // Action to perform on this node

        public PlannerNode(PlannerNode parent, float cost, GoapStateDataDict statesData, AGoapAction action)
        {
            NodeParent = parent;
            NodeCost = cost;
            NodeStates = statesData;
            NodeAction = action;
        }
    }

    /**
    * Calculates new GoapAgent plan by build (recursively) a tree with all actions until we have a path 
    * that meets GoapAgent goal states. Then, get's cheapest path and turn it into a queue of action as the new agent's plan
    */
    public class GoapPlanner
    {
        // Public Methods -----------------------------------------------------------------
        public Queue<AGoapAction> CreateNewAgentPlan(AGoapAgent instigatorAgent, List<AGoapAction> agentActionList, GoapStateDataDict agentGoalTargetStates)
        {
            // First, reset all actions
            foreach(var agentAction in agentActionList)
                agentAction.ResetAction();

            // Get actions that are usable (Some are always usable, others need to test some codition. E.g: Cast spell action)
            List<AGoapAction> usableAgentActionList = new List<AGoapAction>();
            foreach(var agentAction in agentActionList)
            {
                if(agentAction.IsActionUsable(instigatorAgent))
                    usableAgentActionList.Add(agentAction);
            }

            // Start graph leaves list and graph (tree, starting from root)
            // (we store all leaves on the tree we going to build so we can find the cheapest leaf)
            // (The cheapest leaf is a result of the cheapest path on tree => best plan)
            List<PlannerNode> planGraphLeaves = new List<PlannerNode>();
            PlannerNode planGraphRoot = new PlannerNode(null, 0, GoapWorldManager.GoapWorldInstance.GetWorldStates(), null);

            // Create graph (tree) with all possible actions to take, considering action costs, effects, etc.
            bool bFoundAtLeastOnePath = BuildPlanGraph(planGraphRoot, planGraphLeaves, usableAgentActionList, agentGoalTargetStates);

            // If no plan found, return
            if(!bFoundAtLeastOnePath)
            {
                //Debug.LogWarning("No plan found!");
                return null;
            }

            // If found at least one path, get the cheapest one (so we can get the cheapest plan [set of actions])
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

            // However, we may have more than one path with same 'cheapest' cost.
            // Get all of them and get one path randomly
            List<PlannerNode> cheapestNodesList = new List<PlannerNode>();
            foreach(var leaf in planGraphLeaves)
            {
                if(leaf.NodeCost == cheapestNode.NodeCost)
                    cheapestNodesList.Add(leaf);
            }

            // Set cheapestNode as a random path from all with the same 'cheapest' cost value
            Random.InitState ((int)System.DateTime.Now.Ticks);
            int randomIdx = Random.Range(0, cheapestNodesList.Count);
            cheapestNode = cheapestNodesList[randomIdx];

            // Now that we have the cheapest leaf, work it backwards towards root (Node->parent) and build a list with all actions on path
            List<AGoapAction> resultingActionPlanList = new List<AGoapAction>();
            PlannerNode tmpNode = cheapestNode;
            // Until we have a parent on this node, insert it's action on the begining of list
            while(tmpNode != null)
            {
                if(tmpNode.NodeAction != null)
                    resultingActionPlanList.Insert(0, tmpNode.NodeAction);
                tmpNode = tmpNode.NodeParent;
            }

            // Transform list into a queue of actions and return it (this is GoapAgent new action plan)
            Queue<AGoapAction> resultingActionQueue = new Queue<AGoapAction>();
            foreach(var action in resultingActionPlanList)
                resultingActionQueue.Enqueue(action);

            // DEBUG: Debug current chosen action plan
            /* Debug.Log("Plan:");
            foreach(var actionInQueue in resultingActionQueue)
                Debug.Log(actionInQueue.ActionName); 
            Debug.Log("---"); */
        
            return resultingActionQueue;
        }

        // Private Methods ---------------------------------------------------------
        bool BuildPlanGraph(PlannerNode parentNode, List<PlannerNode> graphLeaves, List<AGoapAction> availableActions, GoapStateDataDict agentGoalTargetStates)
        {
            // Aux
            bool bFoundAtLeastOnePath = false;

            // For each available agent on GoapAgent, check if it's 'preconditions' match with 'WorldState states'
            // If it does match, then GoapAgent can execute this action
            // Then, calculate a new temp state that will have 'WorldState states' and action 'Effects'
            // This new temp state will be used recursively on the next call to try another action executing with this resulting state
            // and so on until we find a path that results in goal states match
            foreach(var action in availableActions)
            {
                // Check if Node states fulfill action's preconditions. If it does, then after this ParentNode action execution, 
                // we can execute this action node
                if(action.DoesGivenStateFulfillPreconditions(parentNode.NodeStates))
                {
                    // Calculate new NodeState (this will be previous[parent] node state + actions effects)
                    GoapStateDataDict newStateAfterAction = new GoapStateDataDict();
                    newStateAfterAction.MergeWithGoapStateDict(parentNode.NodeStates);
                    newStateAfterAction.MergeWithGoapStateDict(action.ActionEffects);

                    // Create this new node, consider acumulative cost (parent + action costs) and new state after action execution
                    PlannerNode newGraphPlanNode = new PlannerNode(parentNode, parentNode.NodeCost + action.ActionCost, newStateAfterAction, action);
                
                    // Test if with this action execution, we match a agent goal
                    if(HasGoalBeenAchieved(agentGoalTargetStates, newStateAfterAction))
                    {
                        // If we do, then we have a complete action path into a leaf. 
                        // Add this new node as a leaf and set flag
                        graphLeaves.Add(newGraphPlanNode);
                        bFoundAtLeastOnePath = true;
                        continue;
                    }

                    // If does not meet agent goal states, than continue to build this path recursively
                    // First, calculate new action subset (this subset will have all availableAction on this call - currentAction)
                    List<AGoapAction> newActionSubset = RemoveActionFromList(availableActions, action);

                    // Call recursively to continue to build path
                    bool bFoundPathRecursively = BuildPlanGraph(newGraphPlanNode, graphLeaves, newActionSubset, agentGoalTargetStates);
                    
                    // If we found a path recursively, set flag. 
                    if(bFoundPathRecursively)
                        bFoundAtLeastOnePath = true;
                }
            }

            return bFoundAtLeastOnePath;
        }

        bool HasGoalBeenAchieved(GoapStateDataDict goalTargetStates, GoapStateDataDict statesToCheck)
        {
            // Helper function that checks if 'goalTargetStates' are all present on 'stateToCheck'
            // If it's, that means we have reached a goal, since 'goalTargetStates' are GoapAgent's goal and
            // 'state to check' is the resulting state of a action execution
            foreach(var goalState in goalTargetStates.StateDict)
            {
                // Check if 'statesToCheck' has this goalState
                if(!statesToCheck.StateDict.ContainsKey(goalState.Key))
                    return false;
            }
            return true;
        }

        List<AGoapAction> RemoveActionFromList(List<AGoapAction> targetActionList, AGoapAction actionToRemove)
        {
            // Helper function to remove a action from a list of actions
            // This is uset to calculate the new action subset so we can build the path tree recursively
            List<AGoapAction> newActionList = new List<AGoapAction>();

            foreach(var action in targetActionList)
            {
                // If found action on list, remove it
                if(!action.Equals(actionToRemove))
                    newActionList.Add(action);
            }

            return newActionList;
        }
    }
}
