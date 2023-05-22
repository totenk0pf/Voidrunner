using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Entities.Enemy.Boss {
    public class BossIdle : EnemyState
    {
        [SerializeField] private EnemyState nextState;
        
        [Title("Refs")] 
        [SerializeField] private EnemyMoveRootMotion _moveWithRootMotion;

        public override EnemyState RunCurrentState(){
            if (detected) {
                _moveWithRootMotion.useNavAgent = true;
                return nextState;
            }

            return this;
        }
    }
}
