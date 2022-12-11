using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    public class GoapAIFSMStatePerformAction : AGoapAIFSMStateBase
    {
        // Public Methods --------------------------------------------
        public override void UpdateState(AGoapAgent goapAgent)
        {
            // On 'PerformAction' state, calls a GoapAgent to perform action that it's on Agent action queue
            bool bActionPerformingSuccess = goapAgent.PerformNextActionOnQueue();
        }
    }
}
