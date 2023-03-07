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

public struct MeleeAnimData {
    public readonly string clipStr;
    public readonly float damage;

    public MeleeAnimData(string clipStr, float damage) {
        this.clipStr = clipStr;
        this.damage = damage;
    }
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
    public float Damage => canDamageMod ? (damage + damageModifier) * damageScale : damage;
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

    private bool _canAttack = true;
    private bool _isAttacking = false;
    private bool _isInWindow = false;

    private float _nextSeqTime;
   // private IEnumerator _windowRoutine;
    
    private void IncrementMeleeOrder() => _curMeleeOrder = _curMeleeOrder.Next();

    private void Awake()
    {
        this.AddListener(EventType.WeaponChangedEvent, param => UpdateCurrentWeapon((WeaponManager.WeaponEntry) param));
        this.AddListener(EventType.MeleeAttackBeginEvent, param => OnMeleeAttackBegin());
        this.AddListener(EventType.MeleeAttackEndEvent, param => OnMeleeAttackEnd());
        
        
        //TODO: add a listener to cancel animation of attack on movement (movement, gun) -> StopAllCoroutines()
        if(!MeleeSequence) NCLogger.Log($"Missing Melee Sequence Data", LogLevel.ERROR);

        AssignCollidersData();
        if(!MeleeSequence.ValidateColliders()) NCLogger.Log($"Collider Validation Failed", LogLevel.ERROR);
    }

    private void Start() {
        _curMeleeOrder = MeleeOrder.First;
        _canAttack = true;
        _isAttacking = false;
        _isInWindow = false;
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
        _isInWindow = false;
        
        MeleeSequence.OrderToAttributes[_curMeleeOrder.Next()].EnableCollider();
        
        //StopAllCoroutines here due to spamming melee -> multiple instances of coroutine
        //StopCoroutine(thisRoutine) would not stop all coroutines of same "thisRoutine" method
        StopAllCoroutines();

        var attribute = MeleeSequence.OrderToAttributes[_curMeleeOrder];
        this.FireEvent(EventType.PlayMeleeAttackEvent, new MeleeAnimData(attribute.AnimClipStr, attribute.Damage));
    }

    private void RangedAttackUpdate() {
        if (_curWeaponType != WeaponType.Ranged || !Input.GetMouseButtonDown(0)) return;
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
            _canAttack = false;
        }else {
            _isInWindow = true;
        }
        
        while (curTime < _nextSeqTime) {
            curTime += Time.deltaTime;
            yield return null;
        }
        
        //When exceeds window input time - reset combo chain
        _canAttack = true;
        _isInWindow = false;
        _curMeleeOrder = MeleeOrder.First;
        this.FireEvent(EventType.PlayMeleeAttackEvent, new MeleeAnimData("Idle", 0));
        yield return null;
    }

    private void OnMeleeAttackBegin() {
        ;
    }

    private void OnMeleeAttackEnd() {
        Debug.Log("Anim end");
        _canAttack = true;
        _isAttacking = false;
        StartCoroutine(WaitForInputWindowRoutine());
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

