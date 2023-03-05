using UnityEngine;

namespace Combat
{
    public interface ICombatAnimator
    {
        public void OnAnimationStart();
        public void OnAnimationEnd();
        public void ApplyDamageOnFrame();
        public Animator GetAnimator();
    }
}