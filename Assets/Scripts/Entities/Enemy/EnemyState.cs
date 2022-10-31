using UnityEngine;
using UnityEngine.AI;

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

    private static bool IsInLayerMask(GameObject obj, LayerMask layerMask) {
        return (layerMask.value & (1 << obj.layer)) > 0;
    }
}