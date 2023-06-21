using UnityEngine;

namespace Entities.Enemy.Crawler {
    public class CrawlerAttackBehavior : MonoBehaviour {
        [SerializeField] private CrawlerAttack attack;

        public void OnAttack() {
            if (attack.inRange) attack.DealDamage();
        }

        public void RestartAttack() {
            attack.RestartAttack();
        }
    }
}
