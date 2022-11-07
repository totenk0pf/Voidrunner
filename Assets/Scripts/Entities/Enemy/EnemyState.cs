using Entities.Enemy;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public abstract class EnemyState : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected NavMeshAgent Agent {
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

    [HideInInspector] public GameObject target;
    [HideInInspector] public bool detected;

    public abstract EnemyState RunCurrentState();

    public virtual void OnTriggerEnter(Collider other) {
        if (IsInLayerMask(other.gameObject, playerMask))
        {
            target = other.gameObject;
            detected = true;
        }
    }

    protected static bool IsInLayerMask(GameObject obj, LayerMask layerMask) {
        return (layerMask.value & (1 << obj.layer)) > 0;
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
}