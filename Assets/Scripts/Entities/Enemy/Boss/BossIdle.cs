using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Entities.Enemy.Boss {
    public class BossIdle : EnemyState
    {
        public float minEnemyRange;
        public float maxEnemyRange;

        [Space]
        public float minDelay;
        public float maxDelay;

        private bool _canWalk = true;
        [SerializeField] private EnemyState nextState;
        [SerializeField] private BossAnimData animData;

        public override EnemyState RunCurrentState(){
            if (!animator.GetBool(animData.idleAnim.name)) TriggerAnim(animData.idleAnim);
            if (Agent.remainingDistance <= 0.1f && _canWalk) {
                Agent.SetDestination(GetRandomWayPoint(Random.Range(minEnemyRange, maxEnemyRange)));
                StartCoroutine(Delay(Random.Range(minDelay, maxDelay)));
            }

            if (detected) {
                TriggerAnim(animData.idleAnim);
                return nextState;
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
}
