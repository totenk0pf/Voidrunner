using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [ShowIf("canDamageMod")] public float damageScale = 1;
    [ShowIf("canDamageMod")] public float damageModifier = 0;

    //Getters
    public float NextSeqInputWindow => nextSeqInputWindow;
    public float Damage => damage;
    public float KnockBackForce => knockBackForce;
    public string AnimClipStr => animClipStr;
    
    public void EnableCollider() {
        collider.ResetEnemiesList();
        collider.gameObject.SetActive(true);
    }
    
    public void DisableCollider() {
        collider.ResetEnemiesList();
        collider.gameObject.SetActive(false);
    }
}

public class CombatManager : MonoBehaviour {
    [SerializeField] private MeleeSequenceData MeleeSequence;

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
        
        
        // add a listener to cancel animation of attack on movement=
        if(!MeleeSequence) NCLogger.Log($"Missing Melee Sequence Data", LogLevel.ERROR);

        AssignCollidersData();
        if(!MeleeSequence.ValidateColliders()) NCLogger.Log($"Collider Validation Failed", LogLevel.ERROR);
        
        
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

        if (_curMeleeOrder != _curMeleeOrder.Last()) {
            MeleeSequence.OrderToAttributes[_curMeleeOrder.Next()].EnableCollider();
        }

        var attribute = MeleeSequence.OrderToAttributes[_curMeleeOrder];
        
        switch (_curMeleeOrder) {
            case MeleeOrder.None:
                //event for Melee animator to play animation in its own class
                this.FireEvent(EventType.PlayMeleeAttackEvent, attribute.AnimClipStr);
                //hold that last frame of attack for "nextSeqInputWindow" time (ROUTINE)
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
        
        var windowTime = Time.time + MeleeSequence.OrderToAttributes[_curMeleeOrder].NextSeqInputWindow;
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

    private void AssignCollidersData() {
        var colList = GetComponentsInChildren<MeleeCollider>();
        foreach (var col in colList) {
            foreach (var order in MeleeSequence.OrderToAttributes.Keys.Where(order => col.Order == order)) {
                MeleeSequence.OrderToAttributes[order].collider = col;
            }
        }
    }
   
}

