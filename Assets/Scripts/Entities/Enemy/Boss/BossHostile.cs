using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Enemy.Boss {
    public class BossHostile : EnemyState
    {
        [SerializeField] private EnemyState nextState;
    
        public override EnemyState RunCurrentState() {
            Agent.SetDestination(target.transform.position);
            Agent.isStopped = false;

            if (!(Agent.remainingDistance < 4f)) return this;
            Agent.isStopped = true;
            return nextState;
        }
    }
}
