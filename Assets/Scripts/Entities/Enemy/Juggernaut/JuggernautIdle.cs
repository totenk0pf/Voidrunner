using Entities.Enemy;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class JuggernautIdle : EnemyState
{
    [SerializeField] private EnemyState _nextState;
    [SerializeField] private AnimSerializedData animData;
    
    [Title("Refs")] 
    [SerializeField] private EnemyMoveRootMotion _moveWithRootMotion;

    public override EnemyState RunCurrentState() {
        if (detected) {
            _moveWithRootMotion.useNavAgent = true;
            return _nextState;
        }

        return this;
    }
}
