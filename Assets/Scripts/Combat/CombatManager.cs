using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Core.Logging;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;
public enum MeleeOrder {
    None = -1,
    First = 0, 
    Second = 1,
    Third = 2
}

public class MeleeState : UnitySerializedDictionary<KeyCode, List<GameObject>>
{
    
}

[Serializable]
public class MeleeSequenceNode : SerializedScriptableObject{
    private Dictionary<MeleeOrder, string> order;
    [SerializeField] private float nextSeqInputWindow;
    [SerializeField] private float damage;
    [SerializeField] private float knockBackForce;
    [SerializeField] private MeleeCollider collider;
    public bool canDamageMod;
    [ShowIf("canDamageMod")] public float damageScale = 1;
    [ShowIf("canDamageMod")] public float damageModifier = 0;

    //Getters
    public Dictionary<MeleeOrder, string> Order => order;
    public float NextSeqInputWindow => nextSeqInputWindow;
    public float Damage => damage;
    public float KnockBackForce => knockBackForce;
    public MeleeCollider Collider => collider;
} 

public class CombatManager : MonoBehaviour {
    private readonly string MELEE_SEQUENCE_1 = "MeleeCombo1";
    private readonly string MELEE_SEQUENCE_2 = "MeleeCombo2";
    private readonly string MELEE_SEQUENCE_3 = "MeleeCombo3";
    private readonly string IDLE = "Idle";
    
    private ICombatAnimator MeleeAnimator;
    private ICombatAnimator GunAnimator;

    [SerializeField] private List<MeleeSequenceNode> MeleeSequence;

    private MeleeOrder _curMeleeOrder;
    private WeaponType _curWeaponType;
    private WeaponBase _curWeaponRef;

    private bool _canAttack;
    private bool _isAttacking;
    private bool _isInWindow;

    private void Awake()
    {
        this.AddListener(EventType.WeaponChangedEvent, param => UpdateCurrentWeapon((WeaponManager.WeaponEntry) param));
        this.AddListener(EventType.MeleeAttackBeginEvent, param => OnMeleeAttackBegin());
        this.AddListener(EventType.MeleeAttackEndEvent, param => OnMeleeAttackEnd());
        
        
        // add a listener to cancel animation of attack on movement
        
        // if(!MeleeAnimator) NCLogger.Log($"Missing Animator Reference - MeleeAnimator", LogLevel.WARNING);
        // if(!GunAnimator) NCLogger.Log($"Missing Animator Reference - GunAnimator", LogLevel.WARNING);
    }

    private void Start() {
        _curMeleeOrder = MeleeOrder.None;
    }

    private void Update() {
        if (_canAttack && !_isAttacking) {
            MeleeAttackUpdate();
            RangedAttackUpdate();
        }

    }
    
    private void MeleeAttackUpdate() {
        if (_curWeaponType != WeaponType.Melee || !Input.GetMouseButtonDown(0)) return;
        _canAttack = false;
        _isAttacking = true;
        
        switch (_curMeleeOrder) {
            case MeleeOrder.None:
                
                //do attack
                //hold that last frame of attack for "nextSeqInputWindow" time (ROUTINE)
                //event for Melee animator to play animation in its own class
                _curMeleeOrder = _curMeleeOrder.Next();
                break;
            case MeleeOrder.First:
                break;
            case MeleeOrder.Second:
                break;
            case MeleeOrder.Third:
                break;
            default://Somehow other cases
                throw new EntryPointNotFoundException($"MeleeOrder not recognized");
                break;
        }
        
    }

    private void RangedAttackUpdate() {
        if (_curWeaponType != WeaponType.Ranged || !Input.GetMouseButtonDown(0)) return;
    }
    
    private void UpdateCurrentWeapon(WeaponManager.WeaponEntry entry) {
        _curWeaponType = entry.Type;
        _curWeaponRef = entry.Reference;
    }

    private IEnumerator WaitForInputWindowRoutine()
    {
        if (_curMeleeOrder == MeleeOrder.None) {
            NCLogger.Log($"can't enter wait routine - Invalid Melee Order", LogLevel.WARNING);
            yield break;
        }
        
        var windowTime = Time.time + MeleeSequence[(int)_curMeleeOrder].NextSeqInputWindow;
        _isInWindow = true;
        while (Time.time > windowTime) {
            _isInWindow = false;
            windowTime += Time.deltaTime;
        }
        
        yield return null;
    }

    private void OnMeleeAttackBegin()
    {
        _canAttack = false;
        _isAttacking = true;
    }

    private void OnMeleeAttackEnd()
    {
        _canAttack = true;
        _isAttacking = false;
    }
   
}

