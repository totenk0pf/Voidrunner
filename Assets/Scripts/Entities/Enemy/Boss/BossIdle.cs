using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Entities.Enemy.Boss {
    public class BossIdle : EnemyState
    {
        [SerializeField] private EnemyState nextState;

        public override EnemyState RunCurrentState(){
            return detected ? nextState : this;
        }
    }
}
