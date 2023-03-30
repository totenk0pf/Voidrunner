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
public class MeleeSequenceAttribute {
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
    
    public MeleeSequenceAttribute Clone() {
        return new MeleeSequenceAttribute(animClipStr, nextSeqInputWindow, damage, knockBackForce, collider,
            canDamageMod, damageScale, damageModifier, attackSpeedModifier);
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
public class RangedAttribute
{
    [SerializeField] private float preshotDelay;
    [SerializeField] private float aftershotDelay;

    [SerializeField] private int maxAmmo;
    [SerializeField] private int maxClip;
    [SerializeField] private LayerMask raycastMask;

    public float PreshotDelay => preshotDelay;
    public float AftershotDelay => aftershotDelay;
    public int MaxAmmo => maxAmmo;
    public int MaxClip => maxClip;
    public LayerMask RaycastMask => raycastMask;
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
    [SerializeField] private GameObject meleeVisual;
    [SerializeField] private GameObject rangedVisual;
    [ReadOnly] private WeaponType _curWeaponType;
    [ReadOnly] private WeaponBase _curWeaponRef;
    [Space]
    [ReadOnly] private PlayerMovementController.MovementState _moveState;

    
    private void IncrementMeleeOrder() => _curMeleeOrder = _curMeleeOrder.Next();

    private void Awake() {
        this.AddListener(EventType.WeaponChangedEvent, param => UpdateCurrentWeapon((WeaponManager.WeaponEntry) param));
        this.AddListener(EventType.MeleeAttackBeginEvent, param => OnMeleeAttackBegin());
        this.AddListener(EventType.MeleeAttackEndEvent, param => OnMeleeAttackEnd());
        this.AddListener(EventType.CancelMeleeAttackEvent, param => ResetChain());
        this.AddListener(EventType.SetMovementStateEvent, state => _moveState = (PlayerMovementController.MovementState) state);
        this.AddListener(EventType.WeaponMeleeFiredEvent, param => MeleeAttackUpdate());
        this.AddListener(EventType.WeaponRangedFiredEvent, param => RangedAttackUpdate());

        if(!MeleeSequence) NCLogger.Log($"Missing Melee Sequence Data", LogLevel.ERROR);
        if(!RangedData) NCLogger.Log($"Missing Ranged Data", LogLevel.ERROR);

        if (!meleeVisual) NCLogger.Log($"Missing MeleeVisual", LogLevel.ERROR);
        if (!rangedVisual) NCLogger.Log($"Missing RangedVisual", LogLevel.ERROR);
            
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
        //Clone so that it would not edit in SO data
        this.FireEvent(EventType.PlayMeleeAttackEvent, MeleeSequence.OrderToAttributes[_curMeleeOrder].Clone());
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
    private IEnumerator WaitForInputWindowRoutine()
    {
        Debug.Log($"in window");
        var curTime = Time.time;
        _nextSeqTime = Time.time + MeleeSequence.OrderToAttributes[_curMeleeOrder].NextSeqInputWindow;

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

    private void OnMeleeAttackBegin() {
        ;
    }

    private void OnMeleeAttackEnd() {
        _curWeaponRef.canAttack = true;
        _curWeaponRef.isAttacking = false;
        MeleeSequence.DisableAllColliders();
        StartCoroutine(WaitForInputWindowRoutine());
    }
    
    private void ResetChain() {
        _curWeaponRef.canAttack = true;
        _curWeaponRef.isAttacking = false;
        _isInWindow = false;
        _curMeleeOrder = MeleeOrder.First;
        StopAllCoroutines();
        //TODO: Replace this down the line with "PlayAnimationEvent", not "PlayMeleeAttackEvent"
        this.FireEvent(EventType.PlayMeleeAttackEvent, new MeleeSequenceAttribute("Idle", 0, 0, 0, null));
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

