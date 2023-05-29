using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;
using Core.Debug;
using Core.Events;
using Player.Progression;
using Sirenix.OdinInspector;
using EventType = Core.Events.EventType;

public class PlayerStats : MonoBehaviour
{
    private PlayerProgression progression;
    [SerializeField] private PlayerProgressionData progressionData;

    private void Awake() {
        this.AddListener(EventType.UpdateCombatModifiersEvent, param => UpdateCombatModifiers((MeleeSequenceData) param));
        this.AddListener(EventType.UpdateOxygenModifiersEvent, param => UpdateOxygenModifiers());
        this.AddListener(EventType.EntityDeathEvent, enemyType => progression.AddXP((string) enemyType));
        progression = new PlayerProgression(progressionData);
    }

    private void Start() {
        this.FireEvent(EventType.RefreshModifiersEvent);
        DebugGUI.Instance.AddText(nameof(PlayerStats), "");
    }

    private void Update() {
#if UNITY_EDITOR
        DebugGUI.Instance.UpdateText(nameof(PlayerStats),
        "Progression \n" + 
            $"XP: {progression.CurrentXp}\n" +
            $"XP cap: {progression.CurrentXpCap}\n" +
            $"Level: {progression.CurrentLevel}\n" +
            $"Skill points: {progression.SkillPoints}\n" +
            $"\nSpec types\n" +
            $"Vigor: {progression.CurrentSpec[PlayerProgression.SpecType.Vigor]}\n"+
            $"Endurance: {progression.CurrentSpec[PlayerProgression.SpecType.Endurance]}\n" +
            $"Strength: {progression.CurrentSpec[PlayerProgression.SpecType.Strength]}\n" +
            $"Dexterity: {progression.CurrentSpec[PlayerProgression.SpecType.Dexterity]}"        
            );
#endif
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

    private void UpdateOxygenModifiers() {
        // oxygenCompo.oxygenPool = 100 + 5 * vigor;
    }
}
