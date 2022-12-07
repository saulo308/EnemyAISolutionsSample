using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    public class GoapAIFSM : MonoBehaviour
    {
        // Non-Serializalbe Fields -----------------------------------
        private bool m_boolCanRun = false;
        private AGoapAgent m_targetGoapAgent = null;

        private Stack<AGoapAIFSMStateBase> m_fsmStack = new Stack<AGoapAIFSMStateBase>();

        private GoapAIFSMStateIdle m_idleState = new GoapAIFSMStateIdle();
        private GoapAIFSMStatePerformAction m_performActionState = new GoapAIFSMStatePerformAction();
        private GoapAIFSMStateMoveTo m_moveToState = new GoapAIFSMStateMoveTo();

        // Properties ------------------------------------------------------------
        public GoapAIFSMStateIdle IdleState => m_idleState;
        public GoapAIFSMStatePerformAction PerformActionState => m_performActionState;
        public GoapAIFSMStateMoveTo MoveToState => m_moveToState;

        // Unity Methods -----------------------------------------------
        void Update()
        {
            UpdateCurrentState();
        }

        // Public Methods -----------------------------------------------
        public void InitializeFSM(AGoapAgent targetGoapAgent)
        {
            m_targetGoapAgent = targetGoapAgent;

            m_fsmStack.Push(m_idleState);

            m_boolCanRun = true;
        }

        public AGoapAIFSMStateBase PeekFSMStack()
        {
            return m_fsmStack.Peek();
        }

        public void PushFSMStack(AGoapAIFSMStateBase stateToPush)
        {
            m_fsmStack.Push(stateToPush);
        }

        public AGoapAIFSMStateBase PopFSMStack()
        {
            return m_fsmStack.Pop();
        }

        public void ClearFSMStack()
        {
            m_fsmStack.Clear();
        }

        // Private Methods -------------------------------------------------
        void UpdateCurrentState()
        {
            if(!m_boolCanRun) return;

            AGoapAIFSMStateBase currentState = m_fsmStack.Peek();
		    if (currentState != null)
			    currentState.UpdateState(m_targetGoapAgent);
        }
    }
}
