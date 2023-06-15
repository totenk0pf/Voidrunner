using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using DG.Tweening;
using Entities.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using EventType = Core.Events.EventType;

[Flags]
[Serializable]
public enum EnemyType {
    Flesh,
    Bone,
    Armor
}

public class EnemyBase : EntityBase {
    [TitleGroup("Config")]
    public EnemyType type;
    public float enemyDamage;
    public float enemyHP;
    [SerializeField] private EnemyUI ui;

    [TitleGroup("Refs")]
    [SerializeField] private AnimSerializedData animData;
    [SerializeField] private EnemyMoveRootMotion enemyRootMotion;
    [SerializeField] private Animator animator;

    [TitleGroup("Knockback Settings")] 
    public float stopOffset;
    public Ease knockbackEaseType;
    public LayerMask stopKnockbackLayers;
    
    [TitleGroup("Debug")]
    [ReadOnly] public bool isStunned;
    protected float currentHp;

    private NavMeshAgent _navAgent;
    private EnemyStateMachine _stateMachine;
    private Rigidbody _rb;
    
    private bool _canPull;

    private Tween _currentTween;
    
    //Ground check Attributes
    [ReadOnly] public bool IsGrounded;
    [SerializeField] private float checkDist;
    [SerializeField] private LayerMask groundIgnoreLayer;
    
    #region Public Getters / Caching
    public bool CanPull => _canPull;
    public NavMeshAgent NavMeshAgent {
        get {
            if (!_navAgent) {
                _navAgent = GetComponent<NavMeshAgent>();
            }
            return _navAgent;
        }
    }
    
    public EnemyStateMachine StateMachine {
        get {
            if (!_stateMachine) {
                _stateMachine = GetComponent<EnemyStateMachine>();
            }
            return _stateMachine;
        }
    }
    
    public Rigidbody Rigidbody {
        get {
            if (!_rb) {
                _rb = GetComponent<Rigidbody>();
            }
            return _rb;
        }
    }
    #endregion
    
    private void Start() {
        _canPull = true;
        if (!NavMeshAgent.isOnNavMesh) {
            Debug.LogWarning("No NavMesh is bound to Enemy");
        }
        currentHp = enemyHP;
    }

    private void Update() {
        Die();
        AirborneUpdate();
    }

    #region Update Methods

    public virtual void Die() {
        if (currentHp <= 0)
        {
            var bloods = GetComponentsInChildren<ParticleBase>();
            foreach (var blood in bloods) {
                blood.ForceRelease();
            }
            
            this.FireEvent(EventType.SpawnParticleEnemyDeadEvent, new ParticleCallbackData(Vector3.up, transform.position + Vector3.up));
            Destroy(gameObject);
        }
    }

    public virtual void AirborneUpdate() {
        IsGrounded = Rigidbody.velocity.y < 0.1f;
        if (!IsGrounded) {
            DisablePathfinding();
        } else {
            EnablePathfinding();
        }
    }

    #endregion

    #region Combat Methods
    public virtual void OnGrappled() {
        _canPull = false;
        isStunned = true;
        enemyRootMotion.OnMoveChange(false);
        foreach (var anim in animData.attackAnim) {ResetAnim(anim);}
        animator.SetTrigger(animData.hitAnim[0].name);
        if (_currentTween != null) {
            _currentTween.Kill();
            _currentTween = null;
        }
        DisablePathfinding();
    }
    
    public virtual void OnRelease() {
        _canPull = true;
        isStunned = false;
        enemyRootMotion.OnMoveChange(true);
        EnablePathfinding();
    }

    public virtual void Stun(float duration) {
        StartCoroutine(StunCoroutine(duration));
    }
    
    protected virtual IEnumerator StunCoroutine(float duration) {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        _navAgent.ResetPath();
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

    public virtual void OnKnockback(Vector3 dir, float duration) {
        StartCoroutine(KnockbackCoroutine(dir, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector3 dir, float duration) {
        var isHit =
            Physics.Raycast(
                transform.position,
                -transform.forward,
                out var test,
                10f,
                stopKnockbackLayers);

        if (isHit && test.distance <= stopOffset) yield break;
        
        enemyRootMotion.OnMoveChange(false);
        isStunned = true;

        foreach (var anim in animData.attackAnim) {ResetAnim(anim);}
        animator.SetTrigger(animData.hitAnim[0].name);
        animator.speed = animator.GetCurrentAnimatorStateInfo(0).length / duration;
        _currentTween = transform.root
            .DOMove(dir + transform.position, duration)
            .SetEase(knockbackEaseType)
            .OnUpdate(() => {
                var isHit =
                    Physics.Raycast(
                        transform.position,
                        -transform.forward,
                        out var hit,
                        10f,
                        stopKnockbackLayers);

                if (!isHit) return;
                if (!(hit.distance <= stopOffset)) return;
                
                _currentTween.Kill();
                    
                animator.speed = 1;
                enemyRootMotion.OnMoveChange(true);
                isStunned = false;
            })
            .OnComplete(() => {
                _currentTween = null;
                
                animator.speed = 1;
                enemyRootMotion.OnMoveChange(true);
                isStunned = false; 
            });

        yield return null;
    }
    #endregion

    #region Helpers / Debug
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDist);
    }
    
    private void ResetAnim(AnimParam param) {
        switch (param.type) {
            case AnimatorControllerParameterType.Bool:
                animator.SetBool(param.name, false);
                break;
            case AnimatorControllerParameterType.Trigger:
                animator.ResetTrigger(param.name);
                break;
        }
    }

    protected virtual void EnablePathfinding() {
        if (CanPull && IsGrounded) {
            StateMachine.enabled = true;
            NavMeshAgent.enabled = true;
            Rigidbody.useGravity = false;
        }
    }

    protected virtual void DisablePathfinding() {
        StateMachine.enabled = false;
        NavMeshAgent.enabled = false;
        Rigidbody.useGravity = CanPull;
    }
    #endregion
}
