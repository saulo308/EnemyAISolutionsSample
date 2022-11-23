using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameSharedEventModule
{
    [Serializable, CreateAssetMenu(menuName = "GameSharedEventModule/GameSharedDataEvent/Bool", fileName = "NewBoolSharedDataEvent")]
    public class GameSharedDataEventBool : GameSharedDataEvent<bool>
    {
    }
}