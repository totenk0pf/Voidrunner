using System;
using Core.Patterns;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Audio {
    public class AudioManager : Singleton<AudioManager> {
        public enum Options {
            Pause, 
            Resume,
            Stop,
        }
        [ReadOnly] public AudioSource audioSource;

        private void Awake() {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        public void PlayClip(Vector3 position, AudioClip audioClip) {
            AudioSource.PlayClipAtPoint(audioClip, position);
        }

        public void PlayClip(AudioClip clip) {
            audioSource.PlayOneShot(clip);
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
