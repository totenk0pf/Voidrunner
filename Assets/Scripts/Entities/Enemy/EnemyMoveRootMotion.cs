using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using UnityEngine.AI;
using EventType = Core.Events.EventType;

namespace Entities.Enemy {
    public class EnemyMoveRootMotion : MonoBehaviour {
        private Animator _animator;
        private NavMeshAgent _agent;

        [HideInInspector] public bool canMove = true;

        private Vector2 _lastSmoothDeltaPos;
        private Vector2 _lastVelocity;

        private void Awake() {
            _animator = gameObject.GetComponent<Animator>();
            _agent = transform.parent.GetComponent<NavMeshAgent>();

            _agent.updatePosition = false;
        }

        private void Start() {
            EventDispatcher.Instance.AddListener(EventType.OnPlayerRespawn, _ => {
                canMove = true;
            });
        }

        private void Update() {
            var parent = transform.parent;
            var worldDeltaPosition = _agent.nextPosition - parent.position;
            worldDeltaPosition.y = 0;
            
            //avoid hitting pillars and other crap
            var deltaMagnitude = worldDeltaPosition.magnitude;
            if (deltaMagnitude > _agent.radius / 2f) {
                _agent.nextPosition = Vector3.Lerp(_animator.rootPosition, parent.position,
                    Math.Min(1, Time.deltaTime / 0.1f));
            }
            
            if (worldDeltaPosition.magnitude > _agent.radius) {
                _agent.nextPosition = parent.transform.position + 0.9f * worldDeltaPosition;
            }
        }

        private void OnAnimatorMove() {
            if (!canMove) return;
            var position = _animator.rootPosition;
            position.y = _agent.nextPosition.y;
            transform.parent.position = position;
        }

        public void OnMoveChange(bool state) {
            canMove = state;
        }
    }
}