using Core.Logging;
using UnityEngine;

public class EnemyStateMachine :MonoBehaviour
{
    [SerializeField] private EnemyState _currentState;
    private EnemyBase _enemyBase;
    private EnemyBase enemyBase {
        get {
            if (!_enemyBase) _enemyBase = GetComponent<EnemyBase>();
            return _enemyBase;
        }
    }

    private void Awake() {
        if (_currentState == null) {
            NCLogger.Log($"Initial state not set.", LogLevel.ERROR);
        }
    }

    private void Update() {
        RunStateMachine();
    }

    private void RunStateMachine() {
        EnemyState nextState = enemyBase.isStunned ? _currentState : _currentState.RunCurrentState();
        if (nextState != null) {
            SwitchState(nextState);
        }
    }

    private void SwitchState(EnemyState state) {
        _currentState = state;
    }

    private void OnTriggerEnter(Collider other) {
        _currentState.OnTriggerEnter(other);
    }
}
