using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine :MonoBehaviour
{
    [SerializeField] private EnemyState _currentState;

    private void Start() {
        if (_currentState == null) {
            Debug.LogWarning("No entry state in " + this);
        }
    }

    private void Update() {
        RunStateMachine();
    }

    private void RunStateMachine() {
        EnemyState nextState = _currentState.RunCurrentState();

        if (nextState != null) {
            SwitchState(nextState);
        }
    }

    private void SwitchState(EnemyState state) {
        _currentState = state;
    }
}
