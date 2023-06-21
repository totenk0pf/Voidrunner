using Entities.Enemy;
using Entities.Enemy.Juggernaut;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class JuggernautHostile : EnemyState {
    [Title("Data")]
    [SerializeField] private JuggernautAttack _nextState;
    [SerializeField] private AnimSerializedData animData;

    [HideInInspector] public bool canSwitch = true;
    private AnimParam _currHostileAnim;

    public override EnemyState RunCurrentState() {

        if (_currHostileAnim.name == null) {
            _currHostileAnim = animData.hostileAnim[Random.Range(0, animData.hostileAnim.Count)];
            TriggerAnim(_currHostileAnim);
        }

        Agent.SetDestination(target.transform.position);

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
