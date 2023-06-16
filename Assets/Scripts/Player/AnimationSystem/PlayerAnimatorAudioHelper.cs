using Player.Audio;
using UnityEngine;

namespace Player.AnimationSystem {
    public class PlayerAnimatorAudioHelper : MonoBehaviour {
        public PlayerAudioPlayer player;

        public void PlayAudio(PlayerAudioType type) {
            player.PlayAudio(type);
        }
    }
}