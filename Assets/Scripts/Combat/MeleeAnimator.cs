using System.Collections;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Core.Logging;
using UnityEngine;
using EventType = Core.Events.EventType;

[RequireComponent(typeof(Animator))]
public class MeleeAnimator : MonoBehaviour, ICombatAnimator
{
    private Animator _animator;
    private MeleeSequenceAttribute _meleeAnimData;

    private void Awake() {
        this.AddListener(EventType.PlayMeleeAttackEvent, animData => UpdateAnimAttribute((MeleeSequenceAttribute)animData));
    }

    private void UpdateAnimAttribute(MeleeSequenceAttribute data) {
        _meleeAnimData = data;
        PlayAnimation(data.AnimClipStr);
        //editing the clone -> NOT affect SO data
        _meleeAnimData.AtkSpdModifier = GetClipLength(data.AnimClipStr);
    }

    
    public void PlayAnimation(string clipStr) {
        GetAnimator().speed = _meleeAnimData.AtkSpdModifier;
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
        if(_meleeAnimData != null)
            this.FireEvent(EventType.EnemyDamageEvent, _meleeAnimData);//maybe accompany with enemy instance being damaged?
        else
            NCLogger.Log($"no melee anim data - animData: {_meleeAnimData}");
    }
    #endregion
    
    public Animator GetAnimator() {
        if (!_animator) _animator = GetComponent<Animator>();
        return _animator;
    }

    private float GetClipLength(string name) {
        AnimationClip[] clips = GetAnimator().runtimeAnimatorController.animationClips;
        foreach (var clip in clips) {
            if (clip.name == name)
                return clip.length;
        }
        return 99;//really long so you can tell what's wrong
    }
}
