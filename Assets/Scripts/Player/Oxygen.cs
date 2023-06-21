//#define DEBUG

using Core.Events;
using System.Timers;
using Combat;
using Core.Debug;
using Player.Audio;
using UI;
using UnityEngine;
using EventType = Core.Events.EventType;

public class Oxygen : MonoBehaviour {
    //Oxygen pool player has throughout the game
    public float oxygenPool;
    public float currentOxygen;
    public bool hasDied;

    [Space] public PlayerAudioPlayer audioPlayer;

    private void Start() {
        //Just in case
        if (oxygenPool == 0) Debug.LogWarning("Oxygen Pool might be null");
    }

    private void Update() {
#if DEBUG
        DebugText();
#endif
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReduceOxygen(10f);
        }
    }

    private void DebugText() {
        DebugGUI.Instance.UpdateText(nameof(Oxygen),
            "\nOxygen\n" +
            $"Current: {currentOxygen}\n"
        );
    }

    public void FireUIEvent() {
        EventDispatcher.Instance.FireEvent(EventType.UIBarChangedEvent, new BarUIMsg {
            type  = BarUI.BarType.Oxygen,
            value = currentOxygen / oxygenPool
        });
    }

    public void ReduceOxygen(float amount) {
        if (hasDied) return;
        currentOxygen -= amount;
        if (currentOxygen <= 0) {
            hasDied = true;
            EventDispatcher.Instance.FireEvent(EventType.OnPlayerDie);
            audioPlayer.PlayAudio(PlayerAudioType.PlayerDie);
        }
        else {
            this.FireEvent(EventType.SpawnBloodEvent, new ParticleCallbackData(-transform.forward,
                transform.position + -transform.forward * .5f + transform.up * .6f,
                transform));
            this.FireEvent(EventType.PlayAnimationEvent, new AnimData(PlayerAnimState.HurtByJohnnyCash, 1f));
            this.FireEvent(EventType.CancelAttackEvent, WeaponType.Melee);
            this.FireEvent(EventType.CancelAttackEvent, WeaponType.Ranged);
            this.FireEvent(EventType.CancelGrappleEvent, true);
            audioPlayer.PlayAudio(PlayerAudioType.PlayerHurt);
        }
        FireUIEvent();
    }
}