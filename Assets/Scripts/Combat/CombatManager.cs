using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Core.Logging;
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
public class MeleeSequenceNode {
    [SerializeField] private MeleeOrder order;
    [SerializeField] private float nextSeqInputWindow;
    [SerializeField] private float damage;
    [SerializeField] private float knockBackForce;
    [SerializeField] private MeleeCollider collider;
    public bool canDamageMod;
    [ShowIf("canDamageMod")] public float damageScale = 1;
    [ShowIf("canDamageMod")] public float damageModifier = 0;

    //Getters
    public MeleeOrder Order => order;
    public float NextSeqInputWindow => nextSeqInputWindow;
    public float Damage => damage;
    public float KnockBackForce => knockBackForce;
    public MeleeCollider Collider => collider;
} 

public class CombatManager : MonoBehaviour {
    [SerializeField] private Animator MeleeAnimator;
    [SerializeField] private Animator GunAnimator;

    [SerializeField] private List<MeleeSequenceNode> MeleeSequence;

    private MeleeOrder _curMeleeOrder;
    private WeaponType _curWeaponType;
    private WeaponBase _curWeaponRef;

    private bool _canAttack;
    private bool _isAttacking;
    private void Awake()
    {
        this.AddListener(EventType.WeaponChangedEvent, param => UpdateCurrentWeapon((WeaponManager.WeaponEntry) param));
        // add a listener to cancel animation of attack on movement
        
        if(!MeleeAnimator) NCLogger.Log($"Missing Animator Reference - MeleeAnimator", LogLevel.WARNING);
        if(!GunAnimator) NCLogger.Log($"Missing Animator Reference - GunAnimator", LogLevel.WARNING);
    }

    private void Start() {
        _curMeleeOrder = MeleeOrder.None;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"asdsadasdsa");
            MeleeAnimator.Play("MeleeCombo3");
        }
        if (_canAttack && !_isAttacking) {
            MeleeAttackUpdate();
            RangedAttackUpdate();
        }

    }
    
    private void MeleeAttackUpdate() {
        if (_curWeaponType != WeaponType.Melee || !Input.GetMouseButtonDown(0)) return;
        

    }

    private void RangedAttackUpdate() {
        if (_curWeaponType != WeaponType.Ranged || !Input.GetMouseButtonDown(0)) return;
        
        
    }
    
    private void UpdateCurrentWeapon(WeaponManager.WeaponEntry entry) {
        _curWeaponType = entry.Type;
        _curWeaponRef = entry.Reference;
    }
    

    #region Melee Animation Event Functions

    public void OnDamageWindowStart() {
        //activate collision for damage checking - raycast[]
        ;
    }

    public void OnDamageWindowEnd() {
        ;
    }
    
    #endregion
}

