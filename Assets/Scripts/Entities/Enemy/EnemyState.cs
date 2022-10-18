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

    public abstract EnemyState RunCurrentState();
}