using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Core.Events;
using Core.Logging;
using Extensions;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;
using Random = UnityEngine.Random;

public enum MeleeOrder {
    First = 0, 
    Second = 1,
    Third = 2
}


[Serializable]
public class MeleeSequenceAttribute : IAnimDataConvertable {
    [SerializeField] private PlayerAnimState state;
    //the last combo's window will be the last combo's frame hold time.
    //After it ends, next combo string can be played
    [SerializeField] private float nextSeqInputWindow;
    [SerializeField] private float damage;
    [SerializeField] private float knockBackForce;
    [ReadOnly] public Transform playerTransform;
    [ReadOnly] public MeleeCollider collider;
    public bool canDamageMod;
    [ShowIf("canDamageMod")] [SerializeField] private float damageScale = 1;
    [ShowIf("canDamageMod")] [SerializeField] private float damageModifier = 0;
    [ShowIf("canDamageMod")] [SerializeField] private float attackSpeedModifier = 1;
    public ComboAnimContainer ComboAnim;
    
    
    //Getters
    public float NextSeqInputWindow => nextSeqInputWindow;
    public float Damage => canDamageMod ? (damage + damageModifier) * damageScale : damage;
    public PlayerAnimState State => state;
    public float Knockback => knockBackForce;
    
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
        return new AnimData(State, collider.Enemies, Damage, Knockback, playerTransform, AtkSpdModifier);
    }

    public AnimData CloneToAnimData(Transform transform) {
        return new AnimData(State, collider.Enemies, Damage, Knockback, transform, AtkSpdModifier);
    }
    
    public MeleeSequenceAttribute(PlayerAnimState animState, float seqInputWin, float dmg, float knockForce, MeleeCollider col,
        bool dmgMod = false, float dmgScale = 1, float dmgModifier = 0, float dmgSpeed = 1) {
        state = animState;
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
    [SerializeField] private PlayerAnimState state;
    [SerializeField] private float preshotDelay;
    [SerializeField] private float aftershotDelay;
    [SerializeField] private float reloadTime;

    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackStackCap;
    [ReadOnly] public Transform playerTransform;
    [SerializeField] private float damagePerPellet;
    [SerializeField] private int pelletCount;
    [SerializeField] private float rayCastRange;
    [SerializeField] private float maxSpreadAngle;

    [SerializeField] private int maxAmmo;
    [SerializeField] private int maxClip;
    [SerializeField] private LayerMask raycastMask;

    [ReadOnly] public List<EnemyBase> Enemies;

    public Dictionary<EnemyBase, int> EnemyToCountDict
    {
        get
        {
            var dict = new Dictionary<EnemyBase, int>();
            foreach (var enemy in Enemies)
            {
                if (dict.Keys.Contains(enemy))
                    dict[enemy]++;
                else
                    dict.Add(enemy, 0);
            }

            return dict;
        }
    }
    public bool canDamageMod;
    [ShowIf("canDamageMod")] [SerializeField] private float attackSpeed;
    public float ReloadTime => reloadTime;
    public float Knockback => knockbackForce;
    public PlayerAnimState State => state;
    public float PreshotDelay => preshotDelay;
    public float AftershotDelay => aftershotDelay;
    public int MaxAmmo => maxAmmo;
    public int MaxClip => maxClip;
    public LayerMask RaycastMask => raycastMask;
    public float AtkSpdModifier => canDamageMod ? attackSpeed : 1;
    public int PelletCount => pelletCount;
    public float Range => rayCastRange;
    public float Angle => maxSpreadAngle;
    public AnimData CloneToAnimData() {
        return new AnimData(State, EnemyToCountDict, damagePerPellet, knockbackForce, knockbackStackCap, playerTransform, AtkSpdModifier);
    }
    public AnimData CloneToAnimData(Transform transform) {
        return new AnimData(State, EnemyToCountDict, damagePerPellet, Knockback, knockbackStackCap,  transform, AtkSpdModifier);
    }
}

public class CombatManager : MonoBehaviour
{
    [TitleGroup("Ranged Settings")] 
    [SerializeField] private RangedData RangedData;
    [SerializeField] private Transform firePoint;
   
    
    [TitleGroup("Melee settings")]
    [SerializeField] private MeleeSequenceData MeleeSequence;
    [ReadOnly] private MeleeOrder _curMeleeOrder;
    [ReadOnly] private bool _isInWindow = false;
    [ReadOnly] private float _nextSeqTime;
    
    [TitleGroup("General settings")]
    [ReadOnly] private WeaponEntry _meleeEntry;
    [ReadOnly] private WeaponEntry _rangedEntry;
    [Space]
    [ReadOnly] private PlayerMovementController.MovementState _moveState;
    [ReadOnly] [SerializeField] private PlayerAnimator _playerAnimator;

    private WeaponType _activeWeapon = WeaponType.None;
    private bool _isGrounded;
    private void IncrementMeleeOrder() => _curMeleeOrder = _curMeleeOrder.Next();
    private Coroutine _onHoldInputRoutine;
    
    private void Awake() {
        //Init Ref
        this.AddListener(EventType.InitWeaponRefEvent, param => InitWeaponRef( (List<WeaponEntry>) param));
        //Cancel Anim
        this.AddListener(EventType.CancelAttackEvent, state => CancelWeaponAttack((WeaponType)state));
        //Melee Combo Related
        this.AddListener(EventType.AttackBeginEvent, param => OnAttackBegin());
        this.AddListener(EventType.OnInputWindowHold, param => OnInputWindowHold());
        this.AddListener(EventType.AttackEndEvent, param => OnAttackEnd());
        this.AddListener(EventType.WeaponMeleeFiredEvent, param => MeleeAttack());
        this.AddListener(EventType.NotifyPlayerComboSequenceEvent, param => OnAttackAnimation());
        //Ranged Related
        this.AddListener(EventType.WeaponRangedFiredEvent, param => StartCoroutine(RangedAttackRoutine()));
        //Movement State
        this.AddListener(EventType.SetMovementStateEvent, state => _moveState = (PlayerMovementController.MovementState) state);
        this.AddListener(EventType.ReceiveIsOnGroundEvent, param => _isGrounded = (bool)param);
        //Receive Refs
        this.AddListener(EventType.ReceivePlayerAnimatorEvent, animator => _playerAnimator = (PlayerAnimator) animator);
        
        if(!MeleeSequence) NCLogger.Log($"Missing Melee Sequence Data", LogLevel.ERROR);
        if(!RangedData) NCLogger.Log($"Missing Ranged Data", LogLevel.ERROR);

        AssignCollidersData();
        if(!MeleeSequence.ValidateColliders()) NCLogger.Log($"Collider Validation Failed", LogLevel.ERROR);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(.2f);
        
        if(!firePoint) NCLogger.Log($"firePoint = {firePoint}", LogLevel.ERROR);
        if(!_meleeEntry.reference) NCLogger.Log($"_meleeEntry.reference = {_meleeEntry.reference}", LogLevel.ERROR);
        if(!_rangedEntry.reference) NCLogger.Log($"_rangedEntry.reference = {_rangedEntry.reference}", LogLevel.ERROR);
        _curMeleeOrder = MeleeOrder.First;
        //_curWeaponRef.isAttacking = false;
        _isInWindow = false;
        
        this.FireEvent(EventType.RefreshRangedAttributesEvent, RangedData.Attribute);
        this.FireEvent(EventType.UpdateCombatModifiersEvent, MeleeSequence);
        this.FireEvent(EventType.RequestPlayerAnimatorEvent);
    }
    #region Melee Methods
    private void MeleeAttack() {
        if (!_meleeEntry.reference.canAttack || _meleeEntry.reference.isAttacking) return;
        if (_meleeEntry.type != WeaponType.Melee) return;
        if (_moveState != PlayerMovementController.MovementState.Normal) return;
        this.FireEvent(EventType.RequestIsOnGroundEvent, _activeWeapon);
        if (!_isGrounded) return;
        
        //if(_activeWeapon != WeaponType.Melee) this.FireEvent(EventType.CancelAttackEvent,_activeWeapon);
        this.FireEvent(EventType.CancelAttackEvent,WeaponType.Ranged);
        _activeWeapon = WeaponType.Melee;
        _meleeEntry.reference.canAttack = false;
        _meleeEntry.reference.isAttacking = true; 
        _isInWindow = false;

        MeleeSequence.EnableIsolatedCollider(_curMeleeOrder);

        //StopAllCoroutines here due to spamming melee -> multiple instances of coroutine
        //StopCoroutine(thisRoutine) would not stop all coroutines of same "thisRoutine" method
        StopAllCoroutines();
        //Clone -> not edit in SO data
        var animData = MeleeSequence.OrderToAttributes[_curMeleeOrder].CloneToAnimData();
        this.FireEvent(EventType.UpdateActiveWeaponEvent, _activeWeapon);
        this.FireEvent(EventType.PlayAttackEvent, MeleeSequence.OrderToAttributes[_curMeleeOrder].CloneToAnimData(transform.root));
        this.FireEvent(EventType.StopMovementEvent);
    }

    /// <summary>
    /// Routine To Hold the Last frame of Attack Animation for x amount of time
    /// Indicating the Input Window for Next Combo Chain.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForInputWindowRoutine() {
        var curTime = Time.time;
        _nextSeqTime = Time.time + MeleeSequence.OrderToAttributes[_curMeleeOrder].NextSeqInputWindow;
        
        _playerAnimator.ResetTrigger(MeleeSequence.OrderToAttributes[_curMeleeOrder.Previous()].State);
        
        IncrementMeleeOrder();
        if (_curMeleeOrder == MeleeOrder.First) {
            _isInWindow = false;
            _meleeEntry.reference.canAttack = false;
        }else {
            _isInWindow = true;
        }
        
        while (curTime < _nextSeqTime) {
            curTime += Time.deltaTime;
            yield return null;
        }
        
        NCLogger.Log($"fail the chain");
        //When exceeds window input time - reset combo chain
        if(_activeWeapon != WeaponType.Melee) NCLogger.Log($"_activeWeapon should be Melee when it's {_activeWeapon}", LogLevel.ERROR);
        ResetWeaponAttackState(false, _activeWeapon);
        yield return null;
    }
   
    #endregion
    
    #region Ranged Methods
    private IEnumerator RangedAttackRoutine() {
        if (!_rangedEntry.reference.canAttack || _rangedEntry.reference.isAttacking) yield break;
        if (_rangedEntry.type != WeaponType.Ranged) yield break;
        if (_moveState == PlayerMovementController.MovementState.Grappling) yield break;

        NCLogger.Log($"shooting");
        //if(_activeWeapon != WeaponType.Ranged) this.FireEvent(EventType.CancelAttackEvent, _activeWeapon);
        this.FireEvent(EventType.CancelAttackEvent,WeaponType.Melee);
        _activeWeapon = WeaponType.Ranged;
        _rangedEntry.reference.canAttack = false;
        _rangedEntry.reference.isAttacking = true;
        GetEnemies();
        yield return new WaitForSeconds(RangedData.Attribute.PreshotDelay);
        this.FireEvent(EventType.UpdateActiveWeaponEvent, _activeWeapon);
        this.FireEvent(EventType.PlayAttackEvent, RangedData.Attribute.CloneToAnimData(transform.root));
    }

    private void GetEnemies() {
        var attribute = RangedData.Attribute;
        var origin = firePoint.position;
        Physics physics = new Physics();
        
        
        RangedData.ClearEnemiesCache();
        physics.ConeCastEnemy(ref attribute.Enemies, 
            attribute.PelletCount, 
            firePoint.position,
            firePoint.forward,
            attribute.Range,
            attribute.Angle,
            attribute.RaycastMask);
    }
    #endregion
    
    #region Common Attack Methods

    private void CancelWeaponAttack(WeaponType state)
    {
        //NCLogger.Log($"anim speed before {_playerAnimator.GetAnimator().speed}");
        ResetWeaponAttackState(true, state);
        //NCLogger.Log($"anim speed after {_playerAnimator.GetAnimator().speed}");
    }
    /// <summary>
    /// Reset Attack State for each Weapon
    /// </summary>
    /// <param name="isCancel"></param>
    /// <param name="state"> if state is defined in the param -> it is a Previous State of the current attack
    /// if state is not defined in param -> it's the current state (end of ranged/melee sequence)</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ResetWeaponAttackState(bool isCancel = false, WeaponType state = WeaponType.None)
    {
        if (state == WeaponType.None) state = _activeWeapon;
        switch (state) {
            case WeaponType.None:
                NCLogger.Log($"Reset-ing attack state when it's NONE?", LogLevel.WARNING);
                _playerAnimator.ResumeAnimator();
                
                if(_moveState == PlayerMovementController.MovementState.Dodge)
                    this.FireEvent(EventType.ResumeMovementEvent);
                
                //If canceled due to movement (activeWeapon = NONE), check moveState
                if(_moveState == PlayerMovementController.MovementState.Dodge || !isCancel)
                    this.FireEvent(EventType.PlayAnimationEvent, new AnimData(PlayerAnimState.Idle, 1));
                break;
            case WeaponType.Melee:
                //StopAllCoroutines();
                //if(_onHoldInputRoutine == null) NCLogger.Log($"_onHoldInputRoutine is null", LogLevel.ERROR);
                StopCoroutine(nameof(WaitForInputWindowRoutine));
                _meleeEntry.reference.canAttack = true;
                _meleeEntry.reference.isAttacking = false;
                _isInWindow = false;
                _curMeleeOrder = MeleeOrder.First;
                _playerAnimator.ResumeAnimator();
                
                //dodging when firing gun - do not reset active weapon
                if (_activeWeapon == WeaponType.Ranged && isCancel && _moveState == PlayerMovementController.MovementState.Dodge) {
                    _activeWeapon = WeaponType.Ranged;
                }
                else
                {
                    //If canceled due to movement (activeWeapon = NONE), check moveState
                    if((_activeWeapon == WeaponType.Melee && _moveState == PlayerMovementController.MovementState.Dodge) || !isCancel)
                        this.FireEvent(EventType.PlayAnimationEvent, new AnimData(PlayerAnimState.Idle, 1));
                    _activeWeapon = WeaponType.None;
                }
                
                //if(_moveState != PlayerMovementController.MovementState.Dodge)
                    this.FireEvent(EventType.ResumeMovementEvent);
                
                this.FireEvent(EventType.UpdateActiveWeaponEvent, _activeWeapon);
                
                
                
                break;
            case WeaponType.Ranged:
                _activeWeapon = WeaponType.None;
                StopAllCoroutines();
                _rangedEntry.reference.canAttack = true;
                _rangedEntry.reference.isAttacking = false;
                _playerAnimator.ResumeAnimator();
        
                this.FireEvent(EventType.UpdateActiveWeaponEvent, _activeWeapon);
                
                this.FireEvent(EventType.ResumeMovementEvent);
                
                //If canceled due to movement (activeWeapon = NONE), check moveState
                if(_moveState == PlayerMovementController.MovementState.Dodge || !isCancel)
                    this.FireEvent(EventType.PlayAnimationEvent, new AnimData(PlayerAnimState.Idle, 1));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    #endregion
    
    #region Animation Events
    private void OnAttackBegin() {
        if (_activeWeapon == WeaponType.Melee)
        {
        } else {
            ;
        }
    }

    private void OnAttackAnimation()
    {
        var playerT = transform.root;
        var container = MeleeSequence.OrderToAttributes[_curMeleeOrder].ComboAnim;
        container.transform = playerT;
        container.direction = playerT.forward;
            
        this.FireEvent(EventType.RunPlayerComboSequenceEvent, container);
    }    
    
    private void OnInputWindowHold()
    {
        if (_activeWeapon == WeaponType.Melee) {
            _meleeEntry.reference.canAttack = true;
            _meleeEntry.reference.isAttacking = false;
            MeleeSequence.DisableAllColliders();
            
            if (_curMeleeOrder == MeleeOrder.Third) { _playerAnimator.ResumeAnimator(); } 
            else { _playerAnimator.PauseAnimator(); }
            
            // _onHoldInputRoutine = StartCoroutine(WaitForInputWindowRoutine());
            StartCoroutine(nameof(WaitForInputWindowRoutine));
        } else {
            ;
        }
    }

    private void OnAttackEnd()
    {
        this.FireEvent(EventType.PlayAnimationEvent, new AnimData(PlayerAnimState.Idle, 1));
        if (_activeWeapon == WeaponType.Melee)
            OnAttackEndMelee();
        else if (_activeWeapon == WeaponType.Ranged)
        {
            StartCoroutine(OnAttackEndRangedRoutine());
        }
    }

    private void OnAttackEndMelee()
    {
        
    }
    
    private IEnumerator OnAttackEndRangedRoutine() {
        RangedData.ClearEnemiesCache();
        yield return new WaitForSeconds(RangedData.Attribute.AftershotDelay);
        if(_activeWeapon != WeaponType.Ranged) NCLogger.Log($"_activeWeapon should be RANGED when it's {_activeWeapon}", LogLevel.ERROR);
        ResetWeaponAttackState(false, _activeWeapon);
    }
    #endregion
    
    #region Init Methods

    private void InitWeaponRef(List<WeaponEntry> list)
    {
        foreach (var entry in list)
        {
            switch (entry.type) {
                case WeaponType.Melee:
                    _meleeEntry = entry;
                    break;
                case WeaponType.Ranged:
                    _rangedEntry = entry;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
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
   #endregion
}

