using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class JuggernautIdle : EnemyState
{
    public float minEnemyRange;
    public float maxEnemyRange;

    private bool _canWalk = true;
    private bool _detected = false;
    [SerializeField] private EnemyState _nextState;

    public override EnemyState RunCurrentState() {
        Agent.speed = eBase.enemySpeed;

        if (Agent.remainingDistance <= 0.1f && _canWalk) {
            Agent.SetDestination(GetRandomWayPoint(Random.Range(minEnemyRange, maxEnemyRange)));
        }

        if (_detected) return _nextState;
        return this;
    }

    //radius is range to get a random waypoint
    private Vector3 GetRandomWayPoint(float radius) {
        var randomDir = Random.insideUnitSphere * radius;
        randomDir += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit, radius, 1);

        return hit.position;
    }

    public override void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _detected = true;
        }
    }

    public override void OnTriggerStay(Collider other) {

    }
}
