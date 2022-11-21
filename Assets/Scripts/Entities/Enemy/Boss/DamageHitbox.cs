using System;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;

namespace Entities.Enemy.Boss {
    public class DamageHitbox : MonoBehaviour {
        [TitleGroup("Configs")] public LayerMask playerLayer;
        [SerializeField] private EnemyState _attackState;
        
        private bool _attackExecuted;
        
        private void OnEnable(){
            _attackExecuted = false;
        }
        
        private void OnTriggerEnter(Collider other){
            if (CheckLayerMask.IsInLayerMask(other.gameObject, playerLayer) && !_attackExecuted)
            {
                _attackState.DealDamage();
                _attackExecuted = true;
            }
        }
    }
}
