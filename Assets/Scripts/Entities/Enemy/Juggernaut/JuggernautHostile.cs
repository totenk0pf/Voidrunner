using Entities.Enemy;
using Entities.Enemy.Juggernaut;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class JuggernautHostile : EnemyState {
    [Title("Data")]
    [SerializeField] private JuggernautAttack _nextState;
    [SerializeField] private AnimSerializedData animData;

    [HideInInspector] public bool canSwitch = true;
    private AnimParam _currHostileAnim;
    private bool _canSetAnim = true;

    public override EnemyState RunCurrentState() {
        
        if (NavMesh.SamplePosition(target.transform.position, out var hit, Agent.height / 2, NavMesh.AllAreas)) {
            Agent.SetDestination(target.transform.position);
            _canSetAnim = true;
            
            if (_currHostileAnim.name == null) _currHostileAnim = animData.hostileAnim[Random.Range(0, animData.hostileAnim.Count)];
            if (!animator.GetBool(_currHostileAnim.name)) TriggerAnim(_currHostileAnim);
        }
        
        else {
            if (_canSetAnim) {
                _canSetAnim = false;
                ResetAnim(_currHostileAnim);
                _currHostileAnim.name = null;
            };
        }
        
        if (_nextState.inRange && canSwitch) {
            canSwitch = false;
            Agent.ResetPath();
            TriggerAnim(_currHostileAnim);
            _currHostileAnim.name = null;
            return _nextState;
        }
        
        return this;
    }

    protected override void RestartState() {
        canSwitch = true;
    }
}
