using Player;
using UnityEngine;

namespace Combat
{
    public interface ICombatAnimator : IAnimator
    {
        public void OnAnimationStart();
        public void OnAnimationEnd();
        public void ApplyDamageOnFrame();
    }
}