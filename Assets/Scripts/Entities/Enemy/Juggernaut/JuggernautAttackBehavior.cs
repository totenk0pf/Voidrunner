using System.Collections;
using System.Collections.Generic;
using Entities.Enemy.Juggernaut;
using UnityEngine;

public class JuggernautAttackBehavior : MonoBehaviour {
    [SerializeField] private JuggernautAttack state;

    public void OnAttack() {
        if (state.inRange) {
            state.DealDamage();
        }
    }

    public void ResetAttack() {
        state.ResetAttack();
    }
}
