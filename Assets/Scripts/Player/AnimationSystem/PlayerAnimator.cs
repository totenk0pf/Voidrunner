using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Core.Events;
using Core.Logging;
using Entities.Enemy;
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

[Serializable]
public class AnimParamContainer
{
    [ReadOnly] public HardReferenceAnimData data;
    //public AnimatorControllerParameterType paramType;
    [ValueDropdown("GetParamType", IsUniqueList = true, ExpandAllMenuItems = true, HideChildProperties = true)] [ShowInInspector]
    public AnimParam param;
    
    public int Hash => param.hash;
    public AnimatorControllerParameterType Type => param.type;
    public string Name => param.name;
    
    private IEnumerable GetParamType(){
        if (!data) return new List<AnimParam>();
        return data.animParams.Select(x => new ValueDropdownItem(x.name, x));
    }
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
    [SerializeField] private AnimationParamData animationParamData;
    [SerializeField] private int mainAnimLayer = 0;
    
    private void Awake() {
        this.AddListener(EventType.PlayAnimationEvent, animData => UpdateAnimAttribute((AnimData)animData));
        this.AddListener(EventType.PlayAttackEvent, animData => UpdateAnimAttribute((AnimData)animData));
        this.AddListener(EventType.RequestPlayerAnimatorEvent, param => OnRequestAnimator());
        this.AddListener(EventType.WeaponChangedEvent, param => _curWeaponType = ((WeaponManager.WeaponEntry)param).Type);
        
        if(!animationParamData) NCLogger.Log($"Missing Animation State Data", LogLevel.ERROR);
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
        var param = animationParamData.GetAnimParam(state);
        //NCLogger.Log($"{param.paramName}");
        GetAnimator().speed = _animData.AnimSpeed;
        var id = param.Hash;

        switch (param.Type)
        {
            case AnimatorControllerParameterType.Trigger:
                GetAnimator().SetTrigger(id);
                break;
            case AnimatorControllerParameterType.Float:
                GetAnimator().SetFloat(id, param.param.floatParam);
                break;
            case AnimatorControllerParameterType.Int:
                GetAnimator().SetFloat(id, param.param.intParam);
                break;
            case AnimatorControllerParameterType.Bool:
                GetAnimator().SetBool(id, param.param.boolParam);
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
