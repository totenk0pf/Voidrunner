using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Flags]
[Serializable]
public enum EnemyType {
    Flesh,
    Bone,
    Armor
}

public class EnemyBase : EntityBase {
    public EnemyType type;
    public float enemyDamage; 
    public float enemySpeed;
    public float enemyHP;
    protected float currentHp;

    public bool isStunned;
    private float _currentHP;

    [SerializeField] private EnemyUI ui;
    protected NavMeshAgent navAgent;
    protected EnemyStateMachine stateMachine;
    protected Rigidbody rb;
    private bool _canPull;
    
    //Ground check Attributes
    public bool IsGrounded;
    [SerializeField] private float checkDist;
    [SerializeField] private LayerMask groundIgnoreLayer;
    
    #region Public Getters / Caching
    public bool CanPull => _canPull;
    public NavMeshAgent NavMeshAgent {
        get {
            if (!navAgent) {
                navAgent = GetComponent<NavMeshAgent>();
            }
            return navAgent;
        }
    }
    
    public EnemyStateMachine StateMachine {
        get {
            if (!stateMachine) {
                stateMachine = GetComponent<EnemyStateMachine>();
            }
            return stateMachine;
        }
    }
    
    public Rigidbody Rigidbody {
        get {
            if (!rb) {
                rb = GetComponent<Rigidbody>();
            }
            return rb;
        }
    }
    #endregion
    
    private void Start() {
        _canPull = true;
        NavMeshAgent.speed = enemySpeed;

        if (!NavMeshAgent.isOnNavMesh) {
            Debug.LogWarning("No NavMesh is bound to Enemy");
        }
        currentHp = enemyHP;
    }

    private void Update() {
        Die();
        AirborneUpdate();
    }

    public virtual void AirborneUpdate() {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, checkDist, ~groundIgnoreLayer);
        if (!IsGrounded && (StateMachine || NavMeshAgent)) {
            DisablePathfinding();
        } else {
            EnablePathfinding();
        }
    }

    public virtual void Stun(float duration) {
        StartCoroutine(StunCoroutine(duration));
    }
    
    protected virtual IEnumerator StunCoroutine(float duration) {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        navAgent.ResetPath();
        isStunned = false;
        yield return null;
    }
    
    public virtual void TakeDamage(float amount) {
        currentHp -= amount;
        var scaledValue = currentHp / enemyHP;
        ui.UpdateBar(scaledValue);
    }

    public virtual void TakeTickDamage(float damagePerTick, float interval, int ticks) {
        StartCoroutine(TickDamageCoroutine(damagePerTick, interval, ticks));
    }

    protected virtual IEnumerator TickDamageCoroutine(float damagePerTick, float interval, int ticks) {
        for (int i = 0; i < ticks; i++) {
            TakeDamage(damagePerTick);
            yield return new WaitForSeconds(interval);
        }
        yield return null;
    }

    public virtual void Attack() {
        throw new System.NotImplementedException();
    }

    public virtual void Move(Transform destination) {
        NavMeshAgent.SetDestination(destination.position);
    }
 
    public virtual void Move(Vector3 destination) {
        NavMeshAgent.SetDestination(destination);
    }

    public virtual void Die() {
        if (currentHp <= 0) {
            Destroy(gameObject);
        }
    }
}
