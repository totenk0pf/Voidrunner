using System;
using Core.Patterns;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio {
    public class AudioManager : Singleton<AudioManager> {
        public enum Options {
            Pause, 
            Resume,
            Stop,
        }
       
        [ReadOnly] public AudioSource audioSource;
        [ReadOnly] public AudioMixer audioMixer;

        private AudioMixerGroup[] _effectMixerGroup;
        
        private void Awake() {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioMixer  = Resources.Load<AudioMixer>("Audio/MasterMixer");
            
            //0 index is normal, 1 for boost low volume 
            _effectMixerGroup                = audioMixer.FindMatchingGroups("Effects");
            // audioMixer.outputAudioMixerGroup = _effectMixerGroup[0];
        }
        
        public void PlayClip(Vector3 position, AudioClip audioClip, bool isBoost = false) {
            var audioInst = new GameObject {
                name = "Audio Instance",
                transform = {
                    position = position
                }
            };

            var source = audioInst.AddComponent<AudioSource>();
            source.spatialBlend = 1;
            source.clip = audioClip;
            source.outputAudioMixerGroup = isBoost ? _effectMixerGroup[1] : _effectMixerGroup[0];
            source.Play();
            DOVirtual.DelayedCall(audioClip.length, () => {
                Destroy(audioInst);
            });
        }

        public void PlayClip(AudioClip clip) {
            audioSource.PlayOneShot(clip);
        }
        
        public void PlayClip(AudioClip clip, float vol) {
            audioSource.PlayOneShot(clip, vol);
        }

        public void PlayMusic(AudioClip audioClip) {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        public void HandleMusic(Options option) {
            switch (option) {
                case Options.Pause: 
                    audioSource.Pause();
                    break;
                case Options.Resume: 
                    audioSource.Play();
                    break;
                default: 
                    audioSource.Stop();
                    break;
            }
        }
    }
}
