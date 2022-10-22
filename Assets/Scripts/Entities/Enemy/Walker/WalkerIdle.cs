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

    //dang choi lao ti sua sau
    public Transform player;

    private bool _canWalk = true;

    public override EnemyState RunCurrentState() {
        if (Agent.remainingDistance <= 0.1f && _canWalk) {
            Agent.SetDestination(GetRandomWayPoint(Random.Range(minEnemyRange, maxEnemyRange)));
            StartCoroutine(Delay(Random.Range(minDelay, maxDelay)));
        }

        else if (Vector3.Distance(this.transform.position, player.position) < 4f) {
            Agent.SetDestination(player.position);
            Agent.stoppingDistance = 2f;
        }

        return this;
    }

    //radius is range to get a random waypoint
    private Vector3 GetRandomWayPoint(float radius) {
        var randomDir = Random.insideUnitSphere * radius;
        randomDir += transform.position;

        NavMeshHit hit;
        var finalPos = Vector3.zero; 
    
        if (NavMesh.SamplePosition(randomDir, out hit, radius, 1)) {
            finalPos = hit.position;
        }

        return finalPos;
    }

    IEnumerator Delay(float delay) {
        _canWalk = false;
        yield return new WaitForSeconds(delay);
        _canWalk = true;
    }
}
