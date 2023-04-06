using System.Collections;
using System;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Core.Logging;
using Entities.Enemy;
using Player.AnimationSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;

public enum AnimParamType {
    Trigger,
    Float,
    Int,
    Bool
}

public enum PlayerAnimState {
    MeleeAttack1,
    MeleeAttack2,
    MeleeAttack3,
    RangedAttack,
    Idle,
    Run,
    Dash
}

[Serializable]
public class AnimStateContainer
{
    [EnumToggleButtons]
    public AnimParamType paramType;
    public string paramName;

    public int hash => !string.IsNullOrEmpty(paramName) ? Animator.StringToHash(paramName) : 0;

    // [ShowIf("paramType", AnimParamType.Trigger)]
    // public string triggerParam;
    [ShowIf("paramType", AnimParamType.Float)]
    public float floatParam;
    [ShowIf("paramType", AnimParamType.Int)]
    public int intParam;
    [ShowIf("paramType", AnimParamType.Bool)]
    public bool boolParam;
}

public class AnimData {

    public PlayerAnimState State { get; private set; }
    public float Damage { get; private set; }
    public float AnimSpeed { get; private set; }
    public List<EnemyBase> Enemies { get; private set; }
    public float animDuration;

    public AnimData(PlayerAnimState state, List<EnemyBase> enemies = null, float damage = 0, float animSpeed = 1) {
        this.State = state;
        this.Enemies = enemies;
        this.Damage = damage;
        this.AnimSpeed = animSpeed;
        InitCheck();
    }

    private void InitCheck() {
        if (AnimSpeed < 0.1f)
            NCLogger.Log($"you're pausing {this} animator, make sure the logic is correct", LogLevel.WARNING);
    }
}

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour, IInteractiveAnimator, ICombatAnimator
{
    private Animator _animator;
    private AnimData _animData;
    private WeaponType _curWeaponType;
    [SerializeField] private AnimationStateData animationStateData;
    [SerializeField] private int mainAnimLayer = 0;
    
    private void Awake() {
        this.AddListener(EventType.PlayAnimationEvent, animData => UpdateAnimAttribute((AnimData)animData));
        this.AddListener(EventType.PlayAttackEvent, animData => UpdateAnimAttribute((AnimData)animData));
        this.AddListener(EventType.RequestPlayerAnimatorEvent, param => OnRequestAnimator());
        this.AddListener(EventType.WeaponChangedEvent, param => _curWeaponType = ((WeaponManager.WeaponEntry)param).Type);
        
        if(!animationStateData) NCLogger.Log($"Missing Animation State Data", LogLevel.ERROR);
    }

    private void UpdateAnimAttribute(AnimData data) {
        _animData = data;
        PlayAnimation(_animData.State);
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
        if (_animData == null) {
            NCLogger.Log($"no melee anim data - animData: {_animData}");
            return; }

        _animData.animDuration = CurrentClipLength;
        if (_curWeaponType == WeaponType.Melee) {
            this.FireEvent(EventType.MeleeEnemyDamageEvent, _animData);
        }else if(_curWeaponType == WeaponType.Ranged) {
            this.FireEvent(EventType.RangedEnemyDamageEvent, _animData);
        }
    }

    public void HoldAnimationOnFrame() {
        this.FireEvent(EventType.OnInputWindowHold);
    }
    
    #endregion
 
    #region IAnimator
    public void PlayAnimation(PlayerAnimState state)
    {
        var param = animationStateData.GetAnimParam(state);
        NCLogger.Log($"{param.paramName}");
        GetAnimator().speed = _animData.AnimSpeed;

        switch (param.paramType)
        {
            case AnimParamType.Trigger:
                GetAnimator().SetTrigger(param.hash);
                break;
            case AnimParamType.Float:
                GetAnimator().SetFloat(param.hash, param.floatParam);
                break;
            case AnimParamType.Int:
                GetAnimator().SetFloat(param.hash, param.intParam);
                break;
            case AnimParamType.Bool:
                GetAnimator().SetBool(param.hash, param.boolParam);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public Animator GetAnimator() {
        if (!_animator) _animator = GetComponent<Animator>();
        return _animator;
    }
    #endregion

    private void OnRequestAnimator() {
        this.FireEvent(EventType.ReceivePlayerAnimatorEvent, this);
    }
    public void ResetTrigger(PlayerAnimState state) {
        var param = animationStateData.GetAnimParam(state);
        if (param.paramType != AnimParamType.Trigger) {
            NCLogger.Log($"Not Trigger Param -> Not Reset-able", LogLevel.INFO);
            return;
        }
        GetAnimator().ResetTrigger(param.hash);
    }

    public void PauseAnimator(float speed = 0.01f) {
        GetAnimator().speed = speed;
    }

    public void ResumeAnimator() {
        GetAnimator().speed = 1;
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
