using Audio;
using DG.Tweening;
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
        
        public override void TakeDamage(float amount) {
            base.TakeDamage(amount);
            
            AudioManager.Instance.PlayClip(transform.position, GetAudioClip(EnemyAudioType.GoreLight));
            DOVirtual.DelayedCall(0.1f, () => {
                AudioManager.Instance.PlayClip(transform.position, GetAudioClip(EnemyAudioType.Hit));
            });
        }
    }
}
