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
    private float _damage;

    private void Awake() {
        this.AddListener(EventType.PlayMeleeAttackEvent, animData => UpdateAnimAttribute((MeleeAnimData)animData));
    }

    private void UpdateAnimAttribute(MeleeAnimData data) {
        _damage = data.damage;
        PlayAnimation(data.clipStr);
    }

    
    public void PlayAnimation(string clipStr) {
        GetAnimator().Play(clipStr);
    }
    
    #region Melee Animation Event Functions

    public void OnAnimationStart() {
        this.FireEvent(EventType.MeleeAttackBeginEvent, GetAnimator());
    }

    public void OnAnimationEnd() {
        this.FireEvent(EventType.MeleeAttackEndEvent, GetAnimator());
    }

    public void ApplyDamageOnFrame() {
        //TODO: Implement melee order's collider to damage correct enemies
        this.FireEvent(EventType.EnemyDamageEvent, _damage);//maybe accompany with enemy instance being damaged?
    }
    #endregion
    
    public Animator GetAnimator() {
        if (!_animator) _animator = GetComponent<Animator>();
        return _animator;
    }
}
