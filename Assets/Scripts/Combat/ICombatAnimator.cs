using UnityEngine;

namespace Combat
{
    public interface ICombatAnimator
    {
        public void PlayAnimation(string clipStr);
        public void OnAnimationStart();
        public void OnAnimationEnd();
        public void ApplyDamageOnFrame();
        public Animator GetAnimator();
    }
}