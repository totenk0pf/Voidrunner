using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkerIdle : EnemyState
{
    public float minEnemyRange;
    public float maxEnemyRange;

    [Space]
    public float minDelay;
    public float maxDelay;

    private bool _canWalk = true;
    [SerializeField] private EnemyState _nextState;

    public override EnemyState RunCurrentState() {
        if (Agent.remainingDistance <= 0.1f && _canWalk) {
            Agent.SetDestination(GetRandomWayPoint(Random.Range(minEnemyRange, maxEnemyRange)));
            StartCoroutine(Delay(Random.Range(minDelay, maxDelay)));
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

    IEnumerator Delay(float delay) {
        _canWalk = false;
        yield return new WaitForSeconds(delay);
        _canWalk = true;
    }
}
