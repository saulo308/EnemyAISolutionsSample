using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameSharedEventModule
{
    [Serializable, CreateAssetMenu(menuName = "GameSharedEventModule/GameSharedDataEvent/Float", fileName = "NewFloatSharedDataEvent")]
    public class GameSharedDataEventFloat : GameSharedDataEvent<float>
    {
    }
}