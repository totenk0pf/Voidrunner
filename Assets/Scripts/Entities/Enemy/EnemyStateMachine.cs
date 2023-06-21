using System;
using Core.Events;
using Core.Logging;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;


public class EnemyStateMachine :MonoBehaviour {
    public EnemyState idleState;
    [ReadOnly] public EnemyState currentState;
    [ReadOnly] public bool isRunning = true;
    private EnemyBase _enemyBase;
    private EnemyBase enemyBase {
        get {
            if (!_enemyBase) _enemyBase = GetComponent<EnemyBase>();
            return _enemyBase;
        }
    }

    private void Awake() {
        currentState = idleState;
        if (currentState == null) {
            NCLogger.Log($"Initial state not set.", LogLevel.ERROR);
        }
    }

    private void Start() {
        EventDispatcher.Instance.AddListener(EventType.OnPlayerRespawn, _=>ResetStateMachine());
    }

    private void OnDestroy() {
    }

    private void ResetStateMachine() {
        currentState = idleState;
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
