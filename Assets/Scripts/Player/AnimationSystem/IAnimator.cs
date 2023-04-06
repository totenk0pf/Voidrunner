using UnityEngine;


namespace Player
{
    public interface IAnimator
    { 
        public void PlayAnimation(PlayerAnimState state);
        public Animator GetAnimator();
    }
}