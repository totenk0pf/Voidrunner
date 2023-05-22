using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core {
    public enum TimescaleType {
        Default,
        Slow,
        Pause
    }
    
    [CreateAssetMenu(fileName = "GameManagerData", menuName = "GameManager/Data", order = 0)]
    public class GameManagerData : SerializedScriptableObject {
        [ShowInInspector] public Dictionary<TimescaleType, float> scaleDict = new();
        public float scaleModTime;
    }
}