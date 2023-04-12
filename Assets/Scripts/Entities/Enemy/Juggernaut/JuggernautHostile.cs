using Entities.Enemy;
using Entities.Enemy.Juggernaut;
using UnityEngine;
using UnityEngine.Serialization;

public class JuggernautHostile : EnemyState {
    public float turnSpeed;
    [Space]
    
    [SerializeField] private JuggernautAttack _nextState;
    [SerializeField] private AnimSerializedData animData;

    [HideInInspector] public bool canSwitch = true;
    
    public override EnemyState RunCurrentState() {
        if (!animator.GetBool(animData.hostileAnim[0].name)) TriggerAnim(animData.hostileAnim[0]);
        LookAtTarget(turnSpeed);

        if (_nextState.inRange && canSwitch) {
            canSwitch = false;
            TriggerAnim(animData.hostileAnim[0]);
            return _nextState;
        }
        
        return this;
    }
}
