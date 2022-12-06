using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    public class AgentGoal
    {
        public GoapStateDataDict GoalTargetStates = new GoapStateDataDict();
        public bool IsPersistent = true;
    }

    public abstract class AGoapAgent : MonoBehaviour
    {
        // Serializable Fields -----------------------------------------------
        [SerializeField] private List<AGoapAction> m_agentActionList = new List<AGoapAction>();
        [SerializeField] private List<AgentGoal> m_agentGoalList = new  List<AgentGoal>();

        // Non-Serializable Fields --------------------------------------------- 
        private AGoapAction m_currentAgentAction = null;
        private AgentGoal m_currentAgentGoal = null;

        private Queue<AGoapAction> m_currentActionQueue = new Queue<AGoapAction>();

        // Unity Methods -----------------------------------------------------------
        protected virtual void Awake()
        {
            if(m_agentActionList.Count == 0)
            {
                var agentActions = GetComponents<AGoapAction>();
                foreach(var agentAction in agentActions)
                    m_agentActionList.Add(agentAction);
            }
        }

        protected virtual void Update()
        {

        }
    }
}
