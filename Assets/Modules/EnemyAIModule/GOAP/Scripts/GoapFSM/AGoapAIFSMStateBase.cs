using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    public abstract class AGoapAIFSMStateBase
    {
        public abstract void UpdateState(AGoapAgent goapAgent);
    }
}
