using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public class PlayerStats : MonoBehaviour
{
    //Will add more on the way. 
    [Header("Refs")]
    [SerializeField] private Oxygen oxygenComponent;
    [SerializeField] private PlayerProgession progessionComponent;
    [Space]
    [SerializeField] private MeleeBase meleeBase;

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
        CheckRef(meleeBase);

        level = progessionComponent.level;

        vigor = progessionComponent.vigor;
        endurance = progessionComponent.endurance;
        strength = progessionComponent.strength;
        dexterity = progessionComponent.dexterity;
    }

    private void Start() {
        //Oxygen Modifiers
        oxygenComponent.oxygenPool = 100 + 5 * vigor;

        //Combat Modifiers
        meleeBase.damageScale = 1; //Default 1 haven't implemented damage scaling yet
        meleeBase.damageModifier = meleeBase.damageScale * level;
        meleeBase.attackSpeedModifier = dexterity;
    }

    private void CheckRef<T>(T reference) {
        if (reference == null) {
            Debug.LogError("Missing refs at: " + this);
        }
    }
}
