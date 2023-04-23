using System;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyStateMachine :MonoBehaviour
{
    public EnemyState currentState;
    private EnemyBase _enemyBase;
    private EnemyBase enemyBase {
        get {
            if (!_enemyBase) _enemyBase = GetComponent<EnemyBase>();
            return _enemyBase;
        }
    }

    private void Awake() {
        if (currentState == null) {
            NCLogger.Log($"Initial state not set.", LogLevel.ERROR);
        }
    }

    private void Update() {
        RunStateMachine();
    }

    private void RunStateMachine() {
        EnemyState nextState = enemyBase.isStunned ? currentState : currentState.RunCurrentState();
        if (nextState != null) {
            SwitchState(nextState);
        }
    }

    private void SwitchState(EnemyState state) {
        currentState = state;
    }

    private void OnTriggerEnter(Collider other) {
        currentState.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other) {
        currentState.OnTriggerExit(other);
    }   
}
