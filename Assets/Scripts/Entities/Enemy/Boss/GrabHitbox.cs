using System;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;

namespace Entities.Enemy.Boss {
    public class GrabHitbox : MonoBehaviour {
        [TitleGroup("Configs")] public LayerMask playerLayer;
        public bool grabbed;
    
        private void OnTriggerEnter(Collider other){
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer))
            {
                grabbed = true;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer))
            {
                grabbed = false;
            }
        }
    }
}
