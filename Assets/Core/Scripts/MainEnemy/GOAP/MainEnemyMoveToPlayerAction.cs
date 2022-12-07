using System.Collections;
using System.Collections.Generic;
using EnemyAIModule.GOAP;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyMoveToPlayerAction : AGoapAction
    {
        public override bool RequiresRangeToExecute()
        {
            return false;
        }

        public override bool Perform()
        {
            return true;
        }
    }
}
