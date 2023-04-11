using System.Collections;
using System;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Core.Logging;
using Player.AnimationSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;


public enum PlayerAnimState {
    MeleeAttack1,
    MeleeAttack2,
    MeleeAttack3,
    RangedAttack,
    Idle,
    Run,
    Dash
}

public class AnimData {
    public PlayerAnimState State { get; private set; }
    public float Damage { get; private set; }
    public float Knockback { get; private set; }
    public float KnockbackCap { get; private set; }
    public Transform playerTransform { get; private set; }
    public float AnimSpeed { get; private set; }
    public  Dictionary<EnemyBase, int> EnemiesToCount { get; private set; }
    public List<EnemyBase> Enemies { get; private set; }
    public float animDuration;

    public AnimData(PlayerAnimState state,  Dictionary<EnemyBase, int> enemies = null, float damage = 0, float knockback = 0, float knockbackCap = 0, Transform playerTransform = null, float animSpeed = 1) {
        this.State = state;
        this.EnemiesToCount = enemies;
        this.Damage = damage;
        this.AnimSpeed = animSpeed;
        this.Knockback = knockback;
        this.KnockbackCap = knockbackCap;
        this.playerTransform = playerTransform;
        InitCheck();
    }
    
    public AnimData(PlayerAnimState state,  List<EnemyBase> enemies = null, float damage = 0, float knockback = 0, Transform playerTransform = null, float animSpeed = 1) {
        this.State = state;
        this.Enemies = enemies;
        this.Damage = damage;
        this.AnimSpeed = animSpeed;
        this.Knockback = knockback;
        this.playerTransform = playerTransform;
        InitCheck();
    }

    public AnimData(PlayerAnimState state)
    {
        this.State = state;
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
    private WeaponManager.WeaponEntry _curWeaponEntry;
    private WeaponType _curWeaponType => _curWeaponEntry.Type;
    [SerializeField] private AnimationParamData animationParamData;
    [SerializeField] private int mainAnimLayer = 0;
    
    private void Awake() {
        this.AddListener(EventType.PlayAnimationEvent, animData => UpdateAnimAttribute((AnimData)animData));
        this.AddListener(EventType.PlayAttackEvent, animData => UpdateAnimAttribute((AnimData)animData));
        this.AddListener(EventType.RequestPlayerAnimatorEvent, param => OnRequestAnimator());
        //this.AddListener(EventType.WeaponChangedEvent, param => _curWeaponEntry = ((WeaponManager.WeaponEntry)param));
        this.AddListener(EventType.CancelMeleeAttackEvent, param => SetParam(PlayerAnimState.RangedAttack, false));
        
        if(!animationParamData) NCLogger.Log($"Missing Animation State Data", LogLevel.ERROR);
    }

    private void UpdateAnimAttribute(AnimData data) {
        _animData = data;
        SetParam(_animData.State);
    }

    #region IInteractiveAnimator
    public void OnAnimationStart()
    {
        // if (_animData == null) return;
        // if (_animData.State == PlayerAnimState.Idle)
        // {
        //     _curWeaponEntry.Reference.isAttacking = false;
        // }
        // else
        // {
            if (_curWeaponType == WeaponType.Ranged) {
                SetParam(PlayerAnimState.RangedAttack, false);
            }
            
            this.FireEvent(EventType.AttackBeginEvent, GetAnimator());
        // }
    }

    public void OnAnimationEnd() {
        if (_curWeaponType == WeaponType.Ranged) {
            SetParam(PlayerAnimState.RangedAttack, false);
        }
        
        this.FireEvent(EventType.AttackEndEvent, GetAnimator());
    }
    #endregion
    
    #region ICombatAnimator
    public void ApplyDamageOnFrame() {
        if (_animData == null) {
            NCLogger.Log($"no melee anim data - animData: {_animData}");
            return; }

        if (_animData.State != PlayerAnimState.MeleeAttack1 &&
            _animData.State != PlayerAnimState.MeleeAttack2 &&
            _animData.State != PlayerAnimState.MeleeAttack3 &&
            _animData.State != PlayerAnimState.RangedAttack)
            return;
        
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

    public void RunTweenAnimation() {
        this.FireEvent(EventType.NotifyPlayerComboSequenceEvent);
    }
    
    #endregion
 
    #region IAnimator
    public void SetParam(PlayerAnimState state)
    {
        var param = animationParamData.GetAnimParam(state);
        //NCLogger.Log($"{param.paramName}");
        GetAnimator().speed = _animData?.AnimSpeed ?? 1;
        var id = param.Hash;

        switch (param.Type)
        {
            case AnimatorControllerParameterType.Trigger:
                GetAnimator().SetTrigger(id);
                break;
            case AnimatorControllerParameterType.Float:
                GetAnimator().SetFloat(id, param.floatParam);
                break;
            case AnimatorControllerParameterType.Int:
                GetAnimator().SetFloat(id, param.intParam);
                break;
            case AnimatorControllerParameterType.Bool:
                GetAnimator().SetBool(id, param.boolParam);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (state == PlayerAnimState.Idle) {
            SetParam(PlayerAnimState.RangedAttack, false);
        }
    }

    public void SetParam(PlayerAnimState state, bool value)
    {
        var param = animationParamData.GetAnimParam(state);
        GetAnimator().speed = _animData?.AnimSpeed ?? 1;
        var id = param.Hash;

        if (param.Type == AnimatorControllerParameterType.Bool) {
            GetAnimator().SetBool(id, value);
        } else {
            NCLogger.Log($"Anim State: {state} uses {param.Type} not Bool", LogLevel.ERROR);
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
        var param = animationParamData.GetAnimParam(state);
        if (param.Type != AnimatorControllerParameterType.Trigger) {
            NCLogger.Log($"Not Trigger Param -> Not Reset-able", LogLevel.INFO);
            return;
        }
        GetAnimator().ResetTrigger(param.Hash);
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
