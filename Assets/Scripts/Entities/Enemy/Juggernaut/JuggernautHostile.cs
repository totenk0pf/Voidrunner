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

    public override EnemyState RunCurrentState() {
        if (!animator.GetBool(animData.hostileAnim[0].name)) TriggerAnim(animData.hostileAnim[0]);

        Agent.SetDestination(target.transform.position);

        if (_nextState.inRange && canSwitch) {
            canSwitch = false;
            Agent.ResetPath();
            TriggerAnim(animData.hostileAnim[0]);
            return _nextState;
        }
        
        return this;
    }

    protected override void RestartState() {
        canSwitch = true;
    }
}
