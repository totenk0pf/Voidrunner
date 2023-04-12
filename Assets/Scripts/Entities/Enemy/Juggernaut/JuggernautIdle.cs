using Entities.Enemy;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class JuggernautIdle : EnemyState
{
    [SerializeField] private EnemyState _nextState;
    [SerializeField] private AnimSerializedData animData;

    public override EnemyState RunCurrentState() {
        return detected ? _nextState : this;
    }
}
