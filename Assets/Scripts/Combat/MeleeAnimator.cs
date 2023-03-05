using System.Collections;
using System.Collections.Generic;
using Combat;
using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

[RequireComponent(typeof(Animator))]
public class MeleeAnimator : MonoBehaviour, ICombatAnimator
{
    private Animator _animator;

    #region Melee Animation Event Functions

    public void OnAnimationStart() {
        this.FireEvent(EventType.MeleeAttackBeginEvent, GetAnimator());
    }

    public void OnAnimationEnd() {
        this.FireEvent(EventType.MeleeAttackEndEvent, GetAnimator());
    }

    public void ApplyDamageOnFrame() {
        this.FireEvent(EventType.EnemyDamageEvent);
    }
    #endregion
    
    public Animator GetAnimator() {
        if (!_animator) _animator = GetComponent<Animator>();
        return _animator;
    }
}
