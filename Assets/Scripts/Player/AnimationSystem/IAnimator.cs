using UnityEngine;


namespace Player
{
    public interface IAnimator
    { 
        public void SetParam(PlayerAnimState state);
        public Animator GetAnimator();
    }
}