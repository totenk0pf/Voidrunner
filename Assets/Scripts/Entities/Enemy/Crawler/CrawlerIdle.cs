using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Entities.Enemy.Crawler {
    public class CrawlerIdle : EnemyState {
        public float rotateDuration;
        [Space]
        public float minDelay;
        public float maxDelay;
        [Space]
        [SerializeField] private EnemyMoveRootMotion moveRootMotion;
        [SerializeField] private EnemyState nextState;
    
        private bool _canRotate = true;

        public override EnemyState RunCurrentState() {
            if (_canRotate) {
                StartCoroutine(Rotate());
            }

            if (detected) {
                moveRootMotion.useNavAgent = true;
                return nextState;
            }

            return this;
        }

        private IEnumerator Rotate() {
            _canRotate = false;
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            var parent = transform.root;
            parent.DORotate(parent.rotation * Quaternion.Euler
                (0, Random.Range(parent.rotation.y - 60, parent.rotation.y + 60), 0).eulerAngles, rotateDuration);
            yield return new WaitForSeconds(rotateDuration);
            _canRotate = true;
        }
    }
}

