using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player.Progression {
    [CreateAssetMenu(fileName = "PlayerProgressionData", menuName = "Progression/Data", order = 0)]
    public class PlayerProgressionData : ScriptableObject {
        [TabGroup("Level")] 
        public int defaultLevel; 
        public int maxLevel;
        public int XPGainMod;
    }
}