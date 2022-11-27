using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameSharedEventModule
{
    [Serializable, CreateAssetMenu(menuName = "GameSharedEventModule/GameSharedDataEvent/AnimationEvent", fileName = "NewAnimationEventSharedDataEvent")]
    public class GameSharedDataEventAnimationEvent : GameSharedDataEvent<AnimationEvent>
    {
    }
}