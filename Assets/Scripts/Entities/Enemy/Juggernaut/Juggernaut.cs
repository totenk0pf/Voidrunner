using UnityEngine;

namespace Entities.Enemy.Juggernaut {
    public class Juggernaut : EnemyBase
    {
        public override void OnKnockback(Vector3 dir, float duration) {
            //blank to not have knockback upon being hit by player
        }
        
        public override void OnGrappled() {
            //neko arc
        }
        
        public override void OnRelease() {
            //neko arc
        }
    }
}