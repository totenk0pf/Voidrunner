using System.Collections.Generic;
using Core.Collections;
using Entities.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using StaticClass;

public abstract class EnemyState : MonoBehaviour
{
    protected NavMeshAgent _agent;
    public NavMeshAgent Agent {
        get {
            if (!_agent) _agent = transform.root.GetComponent<NavMeshAgent>();
            return _agent;
        }
    }

    protected EnemyBase _enemyBase;
    protected EnemyBase enemyBase {
        get {
            if (!_enemyBase) _enemyBase = transform.root.GetComponent<EnemyBase>();
            return _enemyBase;
        }
    }

    protected Animator _animator;

    protected Animator animator {
        get{
            if (!_animator) _animator = transform.root.GetComponentInChildren<Animator>();
            return _animator;
        }
    }

    public LayerMask playerMask;

    [ReadOnly] public GameObject target;
    [ReadOnly] public bool detected;

    public abstract EnemyState RunCurrentState();

    public virtual void OnTriggerEnter(Collider other) {
        if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask))
        {
            target = other.gameObject;
            detected = true;
        }
    }

    public virtual void OnTriggerExit(Collider other) {
        if (CheckLayerMask.IsInLayerMask(other.gameObject, playerMask))
        {
            // target = null;
            detected = false;
        }
    }

    protected void TriggerAnim(AnimParam param) {
        switch (param.type) {
            case AnimatorControllerParameterType.Bool:
                animator.SetBool(param.name, !animator.GetBool(param.name));
                break;
            case AnimatorControllerParameterType.Trigger:
                animator.SetTrigger(param.name);
                break;
        }
    }
    
    protected void ResetAnim(AnimParam param) {
        switch (param.type) {
            case AnimatorControllerParameterType.Bool:
                animator.SetBool(param.name, false);
                break;
            case AnimatorControllerParameterType.Trigger:
                animator.ResetTrigger(param.name);
                break;
        }
    }

    public virtual void DealDamage(){
        var oxygenComp = target.GetComponent<Oxygen>();
        oxygenComp.ReduceOxygen(enemyBase.enemyDamage);
    }
    
    protected static float GetPathRemainingDistance(NavMeshAgent navMeshAgent)
    {
        if (navMeshAgent.pathPending ||
            navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
            navMeshAgent.path.corners.Length == 0)
            return -1f;
    
        float distance = 0.0f;
        for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
        }
    
        return distance;
    }

    protected AnimParam GetItemFromMoveList(IEnumerable<EnemyMovelist> movelistData) {
        var weighted = new WeightedArray<AnimParam>();
        foreach (var move in movelistData) {
            weighted.AddElement(move.anim, move.weight);
        }

        return weighted.GetRandomItem();
    }
}