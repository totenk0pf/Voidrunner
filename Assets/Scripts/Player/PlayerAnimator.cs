using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Core.Events;
using EventType = Core.Events.EventType;

namespace Player
{
    public class PlayerAnimator : MonoBehaviour, IAnimator
    {
        private Animator _animator;
    
        private void Awake() {
            this.AddListener(EventType.PlayAnimationEvent, clip => PlayAnimation((string)clip));
        }
    
        public void PlayAnimation(string clipStr) {
            GetAnimator().Play(clipStr);
        }
        
        public Animator GetAnimator() {
            if (!_animator) _animator = GetComponent<Animator>();
            return _animator;
        }
    }
}