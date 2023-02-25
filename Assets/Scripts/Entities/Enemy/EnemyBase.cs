using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : EntityBase
{
    public float enemyDamage; 
    public float enemySpeed;
    public float enemyHP;
    protected float currentHp;

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
        
        ui.healthBar.maxValue = enemyHP;
        ui.healthBar.value = enemyHP;
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
    public virtual void EnablePathfinding() {
        if (CanPull && IsGrounded && (!StateMachine || !NavMeshAgent)) {
            StateMachine.enabled = true;
            NavMeshAgent.enabled = true;
            Rigidbody.useGravity = false;
        }
    }

    public virtual void DisablePathfinding() {
        StateMachine.enabled = false;
        NavMeshAgent.enabled = false;
        Rigidbody.useGravity = CanPull;
    }

    public virtual void OnGrappled() {
        _canPull = false;
        DisablePathfinding();
    }

    public virtual void OnRelease() {
        _canPull = true;
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

    public virtual void TakeDamage(float amount) {
        currentHp -= amount;
        ui.UpdateBar(amount);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDist);
    }
}
