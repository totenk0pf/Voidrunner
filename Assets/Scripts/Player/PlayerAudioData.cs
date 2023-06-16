using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player {

    [Serializable]
    public class PlayerAudio {
        [Serializable]
        public class RangeEffect {
            public List<AudioClip> reload = new ();
            public List<AudioClip> shellDrops = new ();
            public List<AudioClip> pump = new ();
        }
        [Serializable]
        public class MeleeEffect {
            public List<AudioClip> chainsawStartup = new ();
            public List<AudioClip> chainsawIdle = new ();
            public List<AudioClip> leapChainsaw = new ();
        }
        
        [TitleGroup("Combat Audios")] 
        [Header("Range Attacks")]
        public List<AudioClip> rangeAttackAudios = new ();
        public RangeEffect rangeEffectAudios  = new ();
        
        [Header("Melee Attacks")]
        public List<AudioClip> meleeAttackAudios;
        public MeleeEffect meleeEffectAudios;
        
        [TitleGroup("Movement Audios")] 
        [Header("Dash")]
        public List<AudioClip> footstepAudios = new ();
        [Header("Footsteps")]
        public List<AudioClip> dashAudios = new ();
        [Header("Grapple")]
        public List<AudioClip> grappleStartAudios = new ();
        public List<AudioClip> grappleLoopAudios = new ();
        public List<AudioClip> grappleEndAudios = new ();
        
        [TitleGroup("Effect Audios")] 
        public List<AudioClip> playerHurtAudios = new ();
        public List<AudioClip> playerDeathAudios = new ();
    }
    
    [CreateAssetMenu(fileName = "PlayerAudios", menuName = "Audio/PlayerAudios", order = 0)]
    public class PlayerAudioData : ScriptableObject {
        public PlayerAudio playerAudio;
    }
}