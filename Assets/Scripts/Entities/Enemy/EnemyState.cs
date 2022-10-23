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

    protected EnemyBase _eBase;
    protected EnemyBase eBase {
        get {
            if (!_eBase) _eBase = transform.root.GetComponent<EnemyBase>();
            return _eBase;
        }
    }

    public abstract EnemyState RunCurrentState();
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerStay(Collider other);
}