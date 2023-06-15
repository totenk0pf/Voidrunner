using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Enemy {
    public class EnemyMoveRootMotion : MonoBehaviour {
        private Animator _animator;
        private NavMeshAgent _agent;

        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool useNavAgent = false;

        private Vector2 _lastSmoothDeltaPos;
        private Vector2 _lastVelocity;

        private void Awake() {
            _animator = gameObject.GetComponent<Animator>();
            _agent = transform.parent.GetComponent<NavMeshAgent>();

            //Since speed is not accounted in cuz root motion movement 
            //Can set acceleration = angular speed for nearly instant rotate
            _agent.acceleration = _agent.angularSpeed;
        }

        private void Update() {
            var parent = transform.root;
            var worldDeltaPosition = _agent.nextPosition - parent.localPosition;
            worldDeltaPosition.y = 0;

            //avoid hitting pillars and other crap
            var deltaMagnitude = worldDeltaPosition.magnitude;
            if (deltaMagnitude > _agent.radius / 2f) {
                parent.localPosition = Vector3.Lerp(_animator.rootPosition, _agent.nextPosition, Math.Min(1, Time.deltaTime / 0.1f));
            }
        }

        private void OnAnimatorMove() {
            if (_animator && canMove) {
                if (useNavAgent) {
                    if (_agent.updatePosition) _agent.updatePosition = false;
                    
                    var parent = transform.parent;
                    var rootPos = _animator.rootPosition;
                    rootPos.y = _agent.nextPosition.y;
                    
                    parent.localPosition = rootPos;
                    _agent.nextPosition = rootPos;
                }

                else {
                    if (!_agent.updatePosition) _agent.updatePosition = true;
                    transform.parent.localPosition += _animator.deltaPosition;
                }
            }
        }

        public void OnMoveChange(bool state) {
            canMove = state;
            _agent.updatePosition = !state || _agent.updatePosition;
        }
    }
}