using Entities.Enemy;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class JuggernautIdle : EnemyState
{
    public float minEnemyRange;
    public float maxEnemyRange;

    private bool _canWalk = true;
    [SerializeField] private EnemyState _nextState;
    [SerializeField] private AnimSerializedData animData;

    public override EnemyState RunCurrentState(){
        if (animator.GetBool(animData.idleAnim.name)) TriggerAnim(animData.idleAnim);
            
        if (!Agent.pathPending && _canWalk) {
            Agent.SetDestination(GetRandomWayPoint(Random.Range(minEnemyRange, maxEnemyRange)));
            StartCoroutine(Delay(Random.Range(3f, 6f)));
        }

        if (!Agent.pathPending && detected) {
            TriggerAnim(animData.hostileAnim);
            Agent.ResetPath();
            return _nextState;
        }
        return this;
    }

    //radius is range to get a random waypoint
    private Vector3 GetRandomWayPoint(float radius) {
        var randomDir = Random.insideUnitSphere * radius;
        randomDir += transform.position;

        NavMesh.SamplePosition(randomDir, out var hit, radius, 1);

        return hit.position;
    }

    private IEnumerator Delay(float delay) {
        _canWalk = false;
        yield return new WaitForSeconds(delay);
        _canWalk = true;
    }
}
