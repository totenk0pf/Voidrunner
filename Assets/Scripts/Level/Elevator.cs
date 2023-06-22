using System;
using Audio;
using Combat;
using DG.Tweening;
using Sirenix.OdinInspector;
using StaticClass;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace Level {
    public class Elevator : MonoBehaviour {
        [SerializeField] private LayerMask playerLayer;

        [TitleGroup("Elevator Door Mesh")] 
        public GameObject door1;
        public GameObject door2;

        [TitleGroup("Elevator Position")] 
        public Transform pointA;
        public Transform pointB;
        
        [TitleGroup("Anim Config")]
        public float doorCloseDuration;
        public Ease doorEaseType;
        
        [Space]
        public float elevatorDuration;
        public Ease elevatorEaseType;

        [TitleGroup("Audio Config")] 
        public AudioClip elevatorDoorSound;
        public AudioClip elevatorLoopSound;
        public AudioClip elevatorArrivalSound;
        public AudioClip elevatorImpactSound;
        [Space] 
        public AudioSource audioSource;
        public AudioMixer audioMixer;
        public float targetLowPassValue;
        public float maximumVolume;

        private Vector3 _originalDoor1Pos;
        private Vector3 _originalDoor2Pos;

        private Transform _currentPoint;

        private void Start() {
            _currentPoint = pointA;
            _originalDoor1Pos = door1.transform.localPosition;
            _originalDoor2Pos = door2.transform.localPosition;
        }

        private void OnTriggerEnter(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer)) return;
            other.gameObject.transform.SetParent(gameObject.transform);
            var consistentY = other.gameObject.transform.localPosition.y;
            var controller = other.gameObject.GetComponent<PlayerMovementController>();
            var weapons = other.gameObject.GetComponentsInChildren<WeaponBase>();

            foreach (var weapon in weapons) {
                weapon.enabled = false;
            }

            controller.canGravity = false;
            controller.ResetMovement();
            controller.enabled = false;


            other.gameObject.GetComponent<Rigidbody>().useGravity = false;

            var dest = _currentPoint == pointA ? pointB : pointA;
            _currentPoint = _currentPoint == pointA ? pointB : pointA;

            audioMixer.GetFloat("MusicLowPass", out var currentPassValue);
            DOVirtual.Float(currentPassValue, targetLowPassValue, elevatorDoorSound.length, value => {
                audioMixer.SetFloat("MusicLowPass", value);
            });

            AudioManager.Instance.PlayClip(audioSource.transform.position, elevatorDoorSound);
            var s = DOTween.Sequence();
            s
                .Append(door1.transform.DOLocalMoveX(-2, elevatorDoorSound.length).SetEase(doorEaseType))
                .Insert(0, door2.transform.DOLocalMoveX(-2.875f, elevatorDoorSound.length).SetEase(doorEaseType))
                .Append(DOVirtual.DelayedCall(1.2f, null))
                .Append(transform.DOMoveY(dest.position.y, elevatorDuration)
                    .SetEase(elevatorEaseType)
                    .OnStart(() => {
                        audioSource.clip = elevatorLoopSound;
                        audioSource.volume = 0;
                        audioSource.loop = true;
                        DOVirtual.Float(0, maximumVolume, 1.2f, value => {
                            audioSource.volume = value;
                        });
                        audioSource.Play();
                    })
                    .OnComplete(() => {
                        audioSource.Stop();
                        AudioManager.Instance.PlayClip(audioSource.transform.position, elevatorImpactSound);
                        var s2 = DOTween.Sequence();
                        s2.Append(DOVirtual.DelayedCall(elevatorImpactSound.length, () => {
                            AudioManager.Instance.PlayClip(audioSource.transform.position, elevatorArrivalSound);
                        })).Append(DOVirtual.DelayedCall(elevatorArrivalSound.length / 1.2f, () => {
                            AudioManager.Instance.PlayClip(audioSource.transform.position, elevatorDoorSound);
                            door1.transform.DOLocalMoveX(_originalDoor1Pos.x, elevatorDoorSound.length).SetEase(doorEaseType);
                            door2.transform.DOLocalMoveX(_originalDoor2Pos.x, elevatorDoorSound.length).SetEase(doorEaseType);
                            DOVirtual.Float(targetLowPassValue, 22000f, elevatorDoorSound.length, value => {
                                audioMixer.SetFloat("MusicLowPass", value);
                            });
                        }).OnComplete(() => {
                            controller.enabled = true;
                            controller.canGravity = true;
                            
                            foreach (var weapon in weapons) {
                                weapon.enabled = true;
                            }
                        }));
                    }))
                .OnUpdate(() => {
                    other.transform.localPosition
                        = new Vector3(other.transform.localPosition.x, consistentY, other.transform.localPosition.z);
                });
        }
        
        private void OnTriggerExit(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer)) return;
            other.gameObject.GetComponent<Rigidbody>().useGravity = true;
            other.gameObject.transform.SetParent(null);
        }
    }
}
