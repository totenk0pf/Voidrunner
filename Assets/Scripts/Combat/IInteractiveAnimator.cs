﻿using Player;
using UnityEngine;

namespace Combat
{
    public interface IInteractiveAnimator : IAnimator
    {
        public void OnAnimationStart();
        public void OnAnimationEnd();
    }

    public interface ICombatAnimator
    {
        public void ApplyDamageOnFrame();
        public void HoldAnimationOnFrame();
    }
    
    public interface IAnimDataConvertable
    {
        public AnimData CloneToAnimData();
    }
}