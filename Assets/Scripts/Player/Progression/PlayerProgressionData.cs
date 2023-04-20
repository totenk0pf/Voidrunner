using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player.Progression {
    [Serializable]
    public class EnemyExperienceData {
        [ValueDropdown("GetEnemyTags", DropdownTitle = "Select enemy type")]
        public string tag;
        public float gain;
        private IEnumerable<string> GetEnemyTags() {
            return UnityEditorInternal.InternalEditorUtility.tags.Where(x => x.ToLower().Contains("enemy"));
        }
    }
    
    [CreateAssetMenu(fileName = "PlayerProgressionData", menuName = "Progression/Data", order = 0)]
    public class PlayerProgressionData : ScriptableObject {
        [TitleGroup("Level")] 
        public int defaultLevel; 
        public int maxLevel;

        [TitleGroup("Experience")] 
        public float baseXPGain;
        public float XPGainMod;

        [TitleGroup("Enemy settings")] 
        public List<EnemyExperienceData> enemyExperienceData;
    }
}