using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio {
    [Serializable]
    public class EnemyAudio {
        [Header("Idle Audios")]
        public List<AudioClip> idleAudios = new();
        [Header("Movement Audio")]
        public List<AudioClip> movementAudios  = new();
        [Header("Damage Audio")] 
        public List<AudioClip> hitAudios = new();
        [Header("Attack Audio")]
        public List<AudioClip> attackAudios = new();
        
        [Header("Effect Audio")]
        public List<AudioClip> goreLightAudios = new();
        public List<AudioClip> goreHardAudios = new();
        
        [Space]
        public List<AudioClip> footstepAudios = new();
        
    }
    
    [CreateAssetMenu(fileName = "EnemyAudios", menuName = "Audio/EnemyAudios", order = 0)]
    public class EnemyAudioData : ScriptableObject {
        public EnemyAudio enemyAudio;
    }
}