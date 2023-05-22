using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;


[CreateAssetMenu(fileName = "PoolManagerData_SceneX", menuName = "Pool Manager Data", order = 0)]
public class PoolManagerData : SerializedScriptableObject
{
    public Dictionary<EventType, PooledData> eventToPooledData = new();
}
