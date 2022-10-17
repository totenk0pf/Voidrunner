using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Will add more on the way. 
    [Header("Refs")]
    [SerializeField] private Oxygen oxygenComponent;
    [SerializeField] private PlayerMovementController movementControllerComponent;

    [Header("Values and Modifiers")]

    [Header("Oxygen")]
    public float oxygenPool;
    public int regenerateTime;
    public float regenMultiplier = 1;

    [Header("Player Movement")]
    public float speedModifier = 1;
    public float accelModifier = 1;

    private void Start() {
        CheckRef(oxygenComponent);
        oxygenComponent.oxygenPool      = oxygenPool;
        oxygenComponent.regenerateTime  = regenerateTime;
        oxygenComponent.regenMultipiler = regenMultiplier;

        CheckRef(movementControllerComponent);
    }

    private void CheckRef<T>(T reference) {
        if (reference == null) {
            Debug.LogError("Missing refs at: " + this);
        }
    }
}
