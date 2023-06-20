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
using Random = UnityEngine.Random;


public enum PlayerAnimState {
    MeleeAttack1,
    MeleeAttack2,
    MeleeAttack3,
    RangedAttack,

    RunForward,
    RunBackward,
    RunRight,
    RunLeft,
    HaltAllMovement,
    
    DodgeForward,
    DodgeBackward,
    DodgeRight,
    DodgeLeft,
    
    GrapplePoint,
    GrappleEnemy,
    DeGrappleEverything,
    StopAttackChain,
    HurtByJohnnyCash,
    
    UnRunForward,
    UnRunBackward,
    UnRunRight,
    UnRunLeft
}

public class AnimData {
    public PlayerAnimState State { get; private set; }
    public float Damage { get; private set; }
    public float KnockbackRange { get; private set; }
    public float KnockbackDuration { get; private set; }
    public float KnockbackCap { get; private set; }
    public Transform playerTransform { get; private set; }
    public float AnimSpeed { get; private set; }
    public  Dictionary<EnemyBase, int> EnemiesToCount { get; private set; }
    public List<EnemyBase> Enemies { get; private set; }
    public float animDuration;

    public AnimData(PlayerAnimState state,  Dictionary<EnemyBase, int> enemies = null, float damage = 0, float knockbackRange = 0, float knockbackDuration = 0, float knockbackCap = 0, Transform playerTransform = null, float animSpeed = 1) {
        this.State = state;
        this.EnemiesToCount = enemies;
        this.Damage = damage;
        this.AnimSpeed = animSpeed;
        this.KnockbackRange = knockbackRange;
        this.KnockbackDuration = knockbackDuration;
        this.KnockbackCap = knockbackCap;
        this.playerTransform = playerTransform;
        InitCheck();
    }
    
    public AnimData(PlayerAnimState state,  List<EnemyBase> enemies = null, float damage = 0, float knockbackRangeRange = 0, float knockbackDuration = 0, Transform playerTransform = null, float animSpeed = 1) {
        this.State = state;
        this.Enemies = enemies;
        this.Damage = damage;
        this.AnimSpeed = animSpeed;
        this.KnockbackRange = knockbackRangeRange;
        this.KnockbackDuration = knockbackDuration;
        this.playerTransform = playerTransform;
        InitCheck();
    }

    public AnimData(PlayerAnimState state, float animSpeed = 1, Transform playerTransform = null)
    {
        this.State = state;
        this.AnimSpeed = animSpeed;
        this.playerTransform = playerTransform;
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
    //private WeaponEntry _curWeaponEntry;
    //private WeaponType _curWeaponType => _curWeaponEntry.type;
    [SerializeField] private AnimationParamData animationParamData;
    [SerializeField] private int mainAnimLayer = 0;
    private WeaponType _activeType = WeaponType.None;
    private void Awake() {
        this.AddListener(EventType.UpdateActiveWeaponEvent, param => _activeType = (WeaponType) param);
        this.AddListener(EventType.PlayAnimationEvent, animData => UpdateAnimAttribute((AnimData)animData));
        this.AddListener(EventType.PlayAttackEvent, animData => UpdateAnimAttribute((AnimData)animData));
        this.AddListener(EventType.RequestPlayerAnimatorEvent, param => OnRequestAnimator());
        this.AddListener(EventType.CancelAttackEvent, state => OnCancelAttack((WeaponType) state) );
        if(!animationParamData) NCLogger.Log($"Missing Animation State Data", LogLevel.ERROR);
    }

    private void UpdateAnimAttribute(AnimData data) {
        _animData = data;
        if (data.State == PlayerAnimState.HaltAllMovement)
        {
            _animData = data;
        }
        SetParam(_animData.State);
    }

    private void OnCancelAttack(WeaponType state)
    {
        if (state == WeaponType.Ranged && _activeType == WeaponType.Melee)
            SetParam(PlayerAnimState.RangedAttack, false);
    }
    #region IInteractiveAnimator
    public void OnAnimationStart()
    {
        if (_animData is { State: PlayerAnimState.HaltAllMovement })
        {
            this.FireEvent(EventType.CancelAttackEvent, WeaponType.Melee);
            this.FireEvent(EventType.CancelAttackEvent, WeaponType.Ranged);
            return;
        }
        
        if (_activeType == WeaponType.Ranged) {
            SetParam(PlayerAnimState.RangedAttack, false);
        }
        
        this.FireEvent(EventType.AttackBeginEvent, GetAnimator());
    }

    public void OnAnimationEnd()
    {
        if (_animData == null) return;
        
        if (_activeType == WeaponType.Ranged) {
            SetParam(PlayerAnimState.RangedAttack, false);
        }

        if (_animData.State != PlayerAnimState.MeleeAttack1 ||
            _animData.State != PlayerAnimState.MeleeAttack2 ||
            _animData.State != PlayerAnimState.MeleeAttack3)
        {
            this.FireEvent(EventType.SetMovementStateEvent, PlayerMovementController.MovementState.Normal);
            this.FireEvent(EventType.CancelAttackEvent, WeaponType.Melee);
        }
    }

    public void OnAnimationAttackEnd()
    {
        this.FireEvent(EventType.AttackEndEvent, GetAnimator());
    }

    public void RecheckMovement()
    {
        this.FireEvent(EventType.ReUpdateMovementAnimEvent);
    }
    #endregion
    
    #region ICombatAnimator
    public void ApplyDamageOnFrame()
    {
        float length = CurrentClipLength;
        if (_animData == null) {
            NCLogger.Log($"no melee anim data - animData: {_animData}");
            return; }

        if (_animData.State != PlayerAnimState.MeleeAttack1 &&
            _animData.State != PlayerAnimState.MeleeAttack2 &&
            _animData.State != PlayerAnimState.MeleeAttack3 &&
            _animData.State != PlayerAnimState.RangedAttack)
            return;

        //Scale Anim Length with anim speed
        if (_animData.AnimSpeed > 1) {
            var spd = _animData.AnimSpeed - 1;
            length = CurrentClipLength + CurrentClipLength * spd;
            
        }else if (_animData.AnimSpeed < 1) {
            length = CurrentClipLength * _animData.AnimSpeed;
        }

        _animData.animDuration = length;
        if (_activeType == WeaponType.Melee) {
            this.FireEvent(EventType.MeleeEnemyDamageEvent, _animData);
        }else if(_activeType == WeaponType.Ranged) {
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
        var containerList = animationParamData.GetAnimParam(state);
        //NCLogger.Log($"{param.paramName}");
        GetAnimator().speed = _animData?.AnimSpeed ?? 1;
        foreach (var param in containerList)
        {
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

            //this specific if statement was added due to previous system did not support multiple params,
            //delete this when you've updated the Anim Param Data
            if (state == PlayerAnimState.HaltAllMovement) {
                // SetParam(PlayerAnimState.RangedAttack, false);
            }
        }
    }

    //this specific if statement was added due to previous system did not support multiple params,
    //delete this when you've updated the Anim Param Data
    public void SetParam(PlayerAnimState state, bool value)
    {
        var containerList = animationParamData.GetAnimParam(state);
        GetAnimator().speed = _animData?.AnimSpeed ?? 1;

        foreach (var param in containerList)
        {
            var id = param.Hash;

            if (param.Type == AnimatorControllerParameterType.Bool)
            {
                GetAnimator().SetBool(id, value);
            }
            else
            {
                NCLogger.Log($"Anim State: {state} uses {param.Type} not Bool", LogLevel.ERROR);
            }
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
        var containerList = animationParamData.GetAnimParam(state);

        foreach (var param in containerList)
        {
            if (param.Type != AnimatorControllerParameterType.Trigger) {
                NCLogger.Log($"Not Trigger Param -> Not Reset-able", LogLevel.INFO);
                continue;
            }
            GetAnimator().ResetTrigger(param.Hash);
        }
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
