using DG.Tweening;
using UnityEngine;

namespace Audio {
    public class MainMenuAudioHandler : MonoBehaviour {
        [SerializeField] private GameObject bgAudio;
        [SerializeField] private float targetVolume;
        [SerializeField] private float targetPitch;
        [SerializeField] private float targetFrequency;
        [SerializeField] private float targetDuration;
        [SerializeField] private Ease targetEase;

        private float _defaultVolume;
        private float _defaultPitch;
        private float _defaultFrequency;

        private AudioSource _bgSource;
        private AudioLowPassFilter _bgLowPass;

        private void Awake() {
            _bgSource         = bgAudio.GetComponent<AudioSource>();
            _bgLowPass        = bgAudio.GetComponent<AudioLowPassFilter>();
            _defaultVolume    = _bgSource.volume;
            _defaultPitch     = _bgSource.pitch;
            _defaultFrequency = _bgLowPass.cutoffFrequency;
        }

        private void UpdateBackgroundAudio(bool reset) {
            DOTween.To(() => _bgSource.volume,
                       x => _bgSource.volume = x, 
                       reset ? _defaultVolume : targetVolume, 
                       targetDuration).SetEase(targetEase);
            DOTween.To(() => _bgSource.pitch,
                       x => _bgSource.pitch = x, 
                       reset ? _defaultPitch : targetPitch, 
                       targetDuration).SetEase(targetEase);
            DOTween.To(() => _bgLowPass.cutoffFrequency,
                       x => _bgLowPass.cutoffFrequency = x, 
                       reset ? _defaultFrequency : targetFrequency, 
                       targetDuration).SetEase(targetEase);
        }

        public void ResetAudio() => UpdateBackgroundAudio(true);
        public void MuffleAudio() => UpdateBackgroundAudio(false);
    }
}