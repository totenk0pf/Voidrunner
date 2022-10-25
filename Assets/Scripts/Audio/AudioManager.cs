using Core.Logging;
using Core.Patterns;
using UnityEngine;

namespace Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource.GetComponent<AudioSource>();

            if (_audioSource == null) NCLogger.Log("Missing Audio Source", LogLevel.WARNING);
        }

        public void PlayTrack(AudioClip track, float volume = 1f)
        {
            _audioSource.clip = track;
            _audioSource.loop = true;

            _audioSource.Play();
        }

        public void PlayClip(AudioClip clip, Transform position, float volume = 1f)
        {
            AudioSource.PlayClipAtPoint(clip, position.position);
        }

        public void Pause() => _audioSource.Pause(); //Pause Track
        public void Stop() => _audioSource.Stop();   //Stop Track
        public void Continue() { if (_audioSource.clip != null) _audioSource.Play(); } //Continue Track 
    }
}
