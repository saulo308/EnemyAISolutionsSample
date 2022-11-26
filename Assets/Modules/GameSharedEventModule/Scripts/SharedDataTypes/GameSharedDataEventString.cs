using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameSharedEventModule
{
    [Serializable, CreateAssetMenu(menuName = "GameSharedEventModule/GameSharedDataEvent/String", fileName = "NewStringSharedDataEvent")]
    public class GameSharedDataEventString : GameSharedDataEvent<string>
    {
    }
}