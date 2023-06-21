using Audio;
using DG.Tweening;
using UnityEngine;

namespace Entities.Enemy.Boss {
    public class Boss : EnemyBase
    {
        public override void OnKnockback(Vector3 dir, float duration) {
            //blank to not have knockback upon being hit by player
        }
        
        public override void OnGrappled() {
            //shadow wizard money gang
        }
        
        public override void OnRelease() {
            //if you reading this, you are entitled to give me free stuff
        }
        
        public override void TakeDamage(float amount) {
            base.TakeDamage(amount);
            
            AudioManager.Instance.PlayClip(transform.position, GetAudioClip(EnemyAudioType.GoreLight));
            DOVirtual.DelayedCall(Random.Range(0.1f, 0.2f), () => {
                AudioManager.Instance.PlayClip(transform.position, GetAudioClip(EnemyAudioType.Hit));
            });
        }
    }
}
