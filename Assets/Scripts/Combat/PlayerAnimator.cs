using System.Collections;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Core.Logging;
using UnityEngine;
using EventType = Core.Events.EventType;


public class AnimData {
    public string AnimParamStr { get; private set; }
    public float Damage { get; private set; }
    public float AnimSpeed { get; private set; }
    public List<EnemyBase> Enemies { get; private set; }
    public float animDuration;

    public AnimData(string animParamStr, float damage, float animSpeed, List<EnemyBase> enemies) {
        this.AnimParamStr = animParamStr;
        this.Damage = damage;
        this.AnimSpeed = animSpeed;
        this.Enemies = enemies;
    }
}

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour, IInteractiveAnimator, ICombatAnimator
{
    private Animator _animator;
    private AnimData _animData;
    [SerializeField] private int mainAnimLayer = 0;
    
    private void Awake() {
        this.AddListener(EventType.PlayAttackEvent, animData => UpdateAnimAttribute((AnimData)animData));
        this.AddListener(EventType.RequestPlayerAnimatorEvent, param => OnRequestAnimator());
        
    }
    
    private void UpdateAnimAttribute(AnimData data) {
        _animData = data;
        PlayAnimation(_animData.AnimParamStr);
    }

    #region IInteractiveAnimator
    public void OnAnimationStart() {
        this.FireEvent(EventType.AttackBeginEvent, GetAnimator());
    }

    public void OnAnimationEnd() {
        this.FireEvent(EventType.AttackEndEvent, GetAnimator());
    }
    #endregion
    
    #region ICombatAnimator
    public void ApplyDamageOnFrame() {
        //TODO: Implement melee order's collider to damage correct enemies
        if (_animData != null) {
            _animData.animDuration = CurrentClipLength;
            this.FireEvent(EventType.EnemyDamageEvent, _animData);//maybe accompany with enemy instance being damaged?
        }
        else
            NCLogger.Log($"no melee anim data - animData: {_animData}");
    }

    public void HoldAnimationOnFrame() {
        this.FireEvent(EventType.OnInputWindowHold);
    }
    
    #endregion
 
    #region IAnimator
    public void PlayAnimation(string clipStr) {
        GetAnimator().speed = _animData.AnimSpeed;
        GetAnimator().SetTrigger(clipStr);
    }
    
    public Animator GetAnimator() {
        if (!_animator) _animator = GetComponent<Animator>();
        return _animator;
    }
    #endregion

    private void OnRequestAnimator() {
        this.FireEvent(EventType.ReceivePlayerAnimatorEvent, this);
    }
    public void ResetTrigger(string triggerStr) {
        GetAnimator().ResetTrigger(triggerStr);
    }
    
    private float GetClipLength(string name) {
        AnimationClip[] clips = GetAnimator().runtimeAnimatorController.animationClips;
        foreach (var clip in clips) {
            if (clip.name == name)
                return clip.length;
        }
        return 99;//really long so you can tell what's wrong
    }
    private float CurrentClipLength => GetAnimator().GetCurrentAnimatorClipInfo(mainAnimLayer)[0].clip.length;

}
