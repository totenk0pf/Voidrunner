using UnityEngine.AI;
using UnityEngine;

public class JuggernautIdle : EnemyState
{
    public float minEnemyRange;
    public float maxEnemyRange;

    private bool _canWalk = true;
    [SerializeField] private EnemyState _nextState;

    public override EnemyState RunCurrentState() {
        Agent.speed = enemyBase.enemySpeed;

        if (Agent.remainingDistance <= 0.1f && _canWalk) {
            Agent.SetDestination(GetRandomWayPoint(Random.Range(minEnemyRange, maxEnemyRange)));
        }

        if (detected) return _nextState;
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
}
