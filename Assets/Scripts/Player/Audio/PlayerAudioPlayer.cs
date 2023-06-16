using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.Audio {
    public enum PlayerAudioType {
        RangeShoot,
        RangeShellDrops,
        RangeReload,
        RangePump,
        
        MeleeAttack,
        MeleeLeapAttack,
        MeleeStart,
        MeleeIdle,
        
        MovementWalk,
        MovementDash,
        
        GrappleStart,
        GrappleLoop,
        GrappleEnd,
        
        PlayerHurt,
        PlayerDie,
    }
    
    public class PlayerAudioPlayer : MonoBehaviour {
        public PlayerAudioData audioData;
        
        public void PlayAudio(PlayerAudioType type) {
            var clip = GetAudioClipFromType(type);
            AudioManager.Instance.PlayClip(transform.position, clip);
        }
        
        public void PlayAudio(AudioClip clip) {
            AudioManager.Instance.PlayClip(transform.position, clip);
        }

        public AudioClip GetAudioClipFromType(PlayerAudioType type) {
            var list = type switch {
                PlayerAudioType.RangeShoot => audioData.playerAudio.rangeAttackAudios,
                PlayerAudioType.RangeShellDrops => audioData.playerAudio.rangeEffectAudios.shellDrops,
                PlayerAudioType.RangeReload => audioData.playerAudio.rangeEffectAudios.reload,
                PlayerAudioType.RangePump => audioData.playerAudio.rangeEffectAudios.pump,
                
                PlayerAudioType.MeleeAttack => audioData.playerAudio.meleeAttackAudios,
                PlayerAudioType.MeleeLeapAttack => audioData.playerAudio.meleeEffectAudios.leapChainsaw,
                PlayerAudioType.MeleeStart => audioData.playerAudio.meleeEffectAudios.chainsawStartup,
                PlayerAudioType.MeleeIdle => audioData.playerAudio.meleeEffectAudios.chainsawIdle,
                
                PlayerAudioType.MovementWalk => audioData.playerAudio.footstepAudios,
                PlayerAudioType.MovementDash => audioData.playerAudio.dashAudios,
                
                PlayerAudioType.GrappleStart => audioData.playerAudio.grappleStartAudios,
                PlayerAudioType.GrappleLoop => audioData.playerAudio.grappleLoopAudios,
                PlayerAudioType.GrappleEnd => audioData.playerAudio.grappleEndAudios,
                
                PlayerAudioType.PlayerHurt => audioData.playerAudio.playerHurtAudios,
                PlayerAudioType.PlayerDie => audioData.playerAudio.playerDeathAudios,
            };

            return list[Random.Range(0, list.Count)];
        }
    }
}