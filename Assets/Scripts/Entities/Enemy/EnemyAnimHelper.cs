using System;
using System.Collections.Generic;
using Audio;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities.Enemy {
    public class EnemyAnimHelper : MonoBehaviour {
        public EnemyAudioData enemyAudioData;
        private bool _isRepeatAudioPlaying;

        public void PlayAudio(EnemyAudioType type) {
            var audios = GetAudioType(type);
            var clipToPlay = audios[Random.Range(0, audios.Count)];
            AudioManager.Instance.PlayClip(transform.position, clipToPlay);
        }
        
        public void PlayAudioRepeat(EnemyAudioType type) {
            if (_isRepeatAudioPlaying) return;

            _isRepeatAudioPlaying = true;
            var audios = GetAudioType(type);
            var clipToPlay = audios[Random.Range(0, audios.Count)];
            AudioManager.Instance.PlayClip(transform.position, clipToPlay);
            DOVirtual.DelayedCall(Random.Range(1.2f, 2.4f), () => _isRepeatAudioPlaying = false);
        }

        private List<AudioClip> GetAudioType(EnemyAudioType type) {
            return type switch {
                EnemyAudioType.Footsteps => enemyAudioData.enemyAudio.footstepAudios,
                EnemyAudioType.Attack => enemyAudioData.enemyAudio.attackAudios,
                EnemyAudioType.Move => enemyAudioData.enemyAudio.movementAudios,
                EnemyAudioType.Hit => enemyAudioData.enemyAudio.hitAudios,
                EnemyAudioType.GoreLight => enemyAudioData.enemyAudio.goreLightAudios,
                _ => null
            };
        }
    }
}
