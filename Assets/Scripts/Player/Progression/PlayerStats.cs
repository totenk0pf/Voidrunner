using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;
using Core.Events;
using Sirenix.OdinInspector;
using EventType = Core.Events.EventType;

public class PlayerStats : MonoBehaviour
{
    private PlayerProgession progression;
    
    [TitleGroup("Components")]
    [SerializeField] private Oxygen oxygenComponent;

    private void Awake() {
        this.AddListener(EventType.UpdateCombatModifiersEvent, param => UpdateCombatModifiers((MeleeSequenceData) param));
        // this.AddListener(EventType.UpdateOxygenModifiersEvent, param => UpdateOxygenModifiers((Oxygen) param));
        
        progression = new PlayerProgession();
        // level       = progression.currentLevel;
    }

    private void Start() {
        this.FireEvent(EventType.RefreshModifiersEvent);
    }

    private void UpdateCombatModifiers(MeleeSequenceData meleeData)
    {
        //Modifying in SO
        // foreach (var seq in meleeData.OrderToAttributes.Values) {
            // seq.DmgScale = 1; //Default 1 haven't implemented damage scaling yet
            // seq.DmgModifer = seq.DmgScale * level;
            // seq.AtkSpdModifier = dexterity;
        // }
    }

    // private void UpdateOxygenModifiers(Oxygen oxygenCompo)
    // {
        // oxygenCompo.oxygenPool = 100 + 5 * vigor;
    // }
}
