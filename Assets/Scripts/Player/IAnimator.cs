using UnityEngine;


namespace Player
{
    public interface IAnimator
    { 
        public void PlayAnimation(string clipStr);
        public Animator GetAnimator();
    }
}