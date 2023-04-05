using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Core.Events;
using Core.Logging;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using EventType = Core.Events.EventType;

public enum MeleeOrder {
    First = 0, 
    Second = 1,
    Third = 2
}

[Serializable]
public class MeleeSequenceAttribute : IAnimDataConvertable {
    [SerializeField] private string animClipStr;
    //the last combo's window will be the last combo's frame hold time.
    //After it ends, next combo string can be played
    [SerializeField] private float nextSeqInputWindow;
    [SerializeField] private float damage;
    [SerializeField] private float knockBackForce;
    [ReadOnly] public MeleeCollider collider;
    public bool canDamageMod;
    [ShowIf("canDamageMod")] [SerializeField] private float damageScale = 1;
    [ShowIf("canDamageMod")] [SerializeField] private float damageModifier = 0;
    [ShowIf("canDamageMod")] [SerializeField] private float attackSpeedModifier = 1;
    
    //Getters
    public float NextSeqInputWindow => nextSeqInputWindow;
    public float Damage => canDamageMod ? (damage + damageModifier) * damageScale : damage;
    public float KnockBackForce => knockBackForce;
    public string AnimClipStr => animClipStr;

    public float AtkSpdModifier {
        get => canDamageMod ? attackSpeedModifier : ReturnFloatWithLog(1);
        set => attackSpeedModifier = value;
    }

    public float DmgModifer {
        get => canDamageMod ? damageModifier : ReturnFloatWithLog(0);
        set => damageModifier = value;
    }

    public float DmgScale {
        get => canDamageMod ? damageScale : ReturnFloatWithLog(1);
        set => damageScale = value;
    }
    
    public void EnableCollider() {
        collider.ResetEnemiesList();
        collider.gameObject.SetActive(true);
    }
    
    public void DisableCollider() {
        collider.ResetEnemiesList();
        collider.gameObject.SetActive(false);
    }

    public float ReturnFloatWithLog(float val) {
        if(canDamageMod) NCLogger.Log($"Damage Mod: {canDamageMod}, make sure you're not getting a default value", LogLevel.WARNING);
        return val;
    }
    
    public AnimData CloneToAnimData() {
        return new AnimData(animClipStr, Damage, AtkSpdModifier, collider.Enemies);
    }
    
    public MeleeSequenceAttribute(string clipStr, float seqInputWin, float dmg, float knockForce, MeleeCollider col,
        bool dmgMod = false, float dmgScale = 1, float dmgModifier = 0, float dmgSpeed = 1) {
        animClipStr = clipStr;
        nextSeqInputWindow = seqInputWin;
        damage = dmg;
        knockBackForce = knockForce;
        collider = col;
        canDamageMod = dmgMod;
        damageScale = dmgScale;
        damageModifier = dmgScale;
        attackSpeedModifier = dmgSpeed;
    }
}

[Serializable]
public class RangedAttribute : IAnimDataConvertable
{
    [SerializeField] private string fireTriggerStr;
    [SerializeField] private float preshotDelay;
    [SerializeField] private float aftershotDelay;

    [SerializeField] private int maxAmmo;
    [SerializeField] private int maxClip;
    [SerializeField] private LayerMask raycastMask;

    public bool canDamageMod;
    [ShowIf("canDamageMod")] [SerializeField] private float attackSpeed;
    public string TriggerStr => fireTriggerStr;
    public float PreshotDelay => preshotDelay;
    public float AftershotDelay => aftershotDelay;
    public int MaxAmmo => maxAmmo;
    public int MaxClip => maxClip;
    public LayerMask RaycastMask => raycastMask;
    public float AtkSpd => canDamageMod ? attackSpeed : 1;
    public AnimData CloneToAnimData()
    {
        throw new NotImplementedException();
    }
}

public class CombatManager : MonoBehaviour
{
    [TitleGroup("Ranged Settings")] 
    [SerializeField] private RangedData RangedData;
    
    [TitleGroup("Melee settings")]
    [SerializeField] private MeleeSequenceData MeleeSequence;
    [ReadOnly] private MeleeOrder _curMeleeOrder;
    [ReadOnly] private bool _isInWindow = false;
    [ReadOnly] private float _nextSeqTime;
    
    [TitleGroup("General settings")]
    [ReadOnly] private WeaponType _curWeaponType;
    [ReadOnly] private WeaponBase _curWeaponRef;
    [Space]
    [ReadOnly] private PlayerMovementController.MovementState _moveState;
    [ReadOnly] [SerializeField] private PlayerAnimator _playerAnimator;
    
    private void IncrementMeleeOrder() => _curMeleeOrder = _curMeleeOrder.Next();

    private void Awake() {
        //Weapon Switch
        this.AddListener(EventType.WeaponChangedEvent, param => UpdateCurrentWeapon((WeaponManager.WeaponEntry) param));
        //Melee Combo Related
        this.AddListener(EventType.AttackBeginEvent, param => OnAttackBegin());
        this.AddListener(EventType.OnInputWindowHold, param => OnInputWindowHold());
        this.AddListener(EventType.AttackEndEvent, param => OnAttackEnd());
        this.AddListener(EventType.CancelMeleeAttackEvent, param => ResetChain());
        this.AddListener(EventType.WeaponMeleeFiredEvent, param => MeleeAttackUpdate());
        //Ranged Related
        this.AddListener(EventType.WeaponRangedFiredEvent, param => RangedAttackUpdate());
        //Movement State
        this.AddListener(EventType.SetMovementStateEvent, state => _moveState = (PlayerMovementController.MovementState) state);
        //Receive Refs
        this.AddListener(EventType.ReceivePlayerAnimatorEvent, animator => _playerAnimator = (PlayerAnimator) animator);
        
        if(!MeleeSequence) NCLogger.Log($"Missing Melee Sequence Data", LogLevel.ERROR);
        if(!RangedData) NCLogger.Log($"Missing Ranged Data", LogLevel.ERROR);

        AssignCollidersData();
        if(!MeleeSequence.ValidateColliders()) NCLogger.Log($"Collider Validation Failed", LogLevel.ERROR);
    }

    private void Start() {
        if(!_curWeaponRef) NCLogger.Log($"_curWeaponRef = {_curWeaponRef}", LogLevel.ERROR);
        
        _curMeleeOrder = MeleeOrder.First;
        _curWeaponRef.isAttacking = false;
        _isInWindow = false;
        
        this.FireEvent(EventType.RefreshRangedAttributesEvent, RangedData.Attribute);
        this.FireEvent(EventType.UpdateCombatModifiersEvent, MeleeSequence);
        this.FireEvent(EventType.RequestPlayerAnimatorEvent);
    }

    private void MeleeAttackUpdate() {
        if (!_curWeaponRef.canAttack || _curWeaponRef.isAttacking) return;
        if (_curWeaponType != WeaponType.Melee || !Input.GetMouseButtonDown(0)) return;
        if (_moveState != PlayerMovementController.MovementState.Normal) return;
        
        _curWeaponRef.canAttack = false;
        _curWeaponRef.isAttacking = true; 
        _isInWindow = false;

        MeleeSequence.EnableIsolatedCollider(_curMeleeOrder);

        //StopAllCoroutines here due to spamming melee -> multiple instances of coroutine
        //StopCoroutine(thisRoutine) would not stop all coroutines of same "thisRoutine" method
        StopAllCoroutines();
        //Clone -> not edit in SO data
        this.FireEvent(EventType.PlayAttackEvent, MeleeSequence.OrderToAttributes[_curMeleeOrder].CloneToAnimData());
        this.FireEvent(EventType.StopMovementEvent);
    }

    private void RangedAttackUpdate() {
        if (!_curWeaponRef.canAttack || _curWeaponRef.isAttacking) return;
        if (_curWeaponType != WeaponType.Ranged || !Input.GetMouseButtonDown(0)) return;
        if (_moveState != PlayerMovementController.MovementState.Normal) return;
    }
    
    private void UpdateCurrentWeapon(WeaponManager.WeaponEntry entry) {
        _curWeaponType = entry.Type;
        _curWeaponRef = entry.Reference;
    }

    /// <summary>
    /// Routine To Hold the Last frame of Attack Animation for x amount of time
    /// Indicating the Input Window for Next Combo Chain.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForInputWindowRoutine() {
        var curTime = Time.time;
        _nextSeqTime = Time.time + MeleeSequence.OrderToAttributes[_curMeleeOrder].NextSeqInputWindow;
        
        _playerAnimator.ResetTrigger(MeleeSequence.OrderToAttributes[_curMeleeOrder.Previous()].AnimClipStr);
        
        IncrementMeleeOrder();
        if (_curMeleeOrder == MeleeOrder.First) {
            _isInWindow = false;
            _curWeaponRef.canAttack = false;
        }else {
            _isInWindow = true;
        }
        
        while (curTime < _nextSeqTime) {
            curTime += Time.deltaTime;
            yield return null;
        }
        
        //When exceeds window input time - reset combo chain
        ResetChain();
        yield return null;
    }

    private void OnAttackBegin() {
        if (_curWeaponType == WeaponType.Melee)
        {
        } else {
            ;
        }
    }

    private void OnInputWindowHold()
    {
        if (_curWeaponType == WeaponType.Melee) {
            _curWeaponRef.canAttack = true;
             _curWeaponRef.isAttacking = false;
            MeleeSequence.DisableAllColliders();
            
            if (_curMeleeOrder == MeleeOrder.Third) { _playerAnimator.ResumeAnimator(); } 
            else { _playerAnimator.PauseAnimator(); }
            
            StartCoroutine(WaitForInputWindowRoutine());
        } else {
            ;
        }
    }

    private void OnAttackEnd() {
      
    }
    
    private void ResetChain() {
        _curWeaponRef.canAttack = true;
        _curWeaponRef.isAttacking = false;
        _isInWindow = false;
        _curMeleeOrder = MeleeOrder.First;
        _playerAnimator.ResumeAnimator();
        StopAllCoroutines();
        //TODO: Replace this down the line with "PlayAnimationEvent", not "PlayMeleeAttackEvent"
        this.FireEvent(EventType.PlayAttackEvent, new AnimData("Idle", 0, 1, null));
        this.FireEvent(EventType.ResumeMovementEvent);
    }
    
    private void AssignCollidersData() {
        var colList = GetComponentsInChildren<MeleeCollider>();
        foreach (var col in colList) {
            foreach (var order in MeleeSequence.OrderToAttributes.Keys) {
                if (col.Order == order) {
                    var seqCol = MeleeSequence.OrderToAttributes[order].collider;
                    if(seqCol) NCLogger.Log($"Collider at {seqCol.gameObject.name} is assigned, ", LogLevel.WARNING);
                    MeleeSequence.OrderToAttributes[order].collider = col;
                }
            }
        }
    }
   
}

