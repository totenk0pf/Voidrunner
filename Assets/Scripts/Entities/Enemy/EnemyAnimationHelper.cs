using Entities.Enemy.Boss;
using UnityEngine;

namespace Entities.Enemy {
    public class EnemyAnimationHelper : MonoBehaviour {
        [SerializeField] private BossAttack attackState;

        public void DealDamage() {
            attackState.DealDamage();
        }
    }
}