using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Will add more on the way. 
    [Header("Refs")]
    [SerializeField] private Oxygen oxygenComponent;
    [SerializeField] private PlayerProgession progessionComponent;

    [Space]
    public int vigor;
    public int endurance;
    public int strength;
    public int dexterity;

    [Space]
    public int level;

    private void Awake() {
        CheckRef(oxygenComponent);
        CheckRef(progessionComponent);

        level = progessionComponent.level;

        vigor = progessionComponent.vigor;
        endurance = progessionComponent.endurance;
        strength = progessionComponent.strength;
        dexterity = progessionComponent.dexterity;
    }

    private void Start() {
        oxygenComponent.oxygenPool = 100 + 5 * vigor;
    }

    private void CheckRef<T>(T reference) {
        if (reference == null) {
            Debug.LogError("Missing refs at: " + this);
        }
    }
}
