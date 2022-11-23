using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameSharedEventModule
{
    [Serializable, CreateAssetMenu(menuName = "GameSharedEventModule/GameSharedDataEvent/Integer", fileName = "NewIntegerSharedDataEvent")]
    public class GameSharedDataEventInt : GameSharedDataEvent<int>
    {
    }
}