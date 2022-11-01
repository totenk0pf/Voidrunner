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

        public override EnemyState RunCurrentState() {
            if (Agent.remainingDistance <= 0.1f && _canWalk) {
                Agent.SetDestination(GetRandomWayPoint(Random.Range(minEnemyRange, maxEnemyRange)));
                StartCoroutine(Delay(Random.Range(minDelay, maxDelay)));
            }

            if (detected) return nextState;
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
