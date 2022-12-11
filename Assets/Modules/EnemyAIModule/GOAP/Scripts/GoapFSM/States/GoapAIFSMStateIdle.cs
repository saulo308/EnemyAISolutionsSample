using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    public class GoapAIFSMStateIdle : AGoapAIFSMStateBase
    {
        // Public Methods --------------------------------------------
        public override void UpdateState(AGoapAgent goapAgent)
        {
            // On 'idle' state, calculates a new plan for agent
            bool bFoundPlan = goapAgent.RequestNewAgentPlan();
        }
    }
}
