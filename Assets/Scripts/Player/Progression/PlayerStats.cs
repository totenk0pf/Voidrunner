//#define DEBUG

using UnityEngine;
using Combat;
using Core.Debug;
using Core.Events;
using Player.Progression;
using Sirenix.OdinInspector;
using EventType = Core.Events.EventType;

public class PlayerStats : MonoBehaviour
{
    private PlayerProgression _progression;
    [SerializeField] private PlayerProgressionData progressionData;

    private void Awake() {
        this.AddListener(EventType.UpdateCombatModifiersEvent, param => UpdateCombatModifiers((MeleeSequenceData) param));
        // this.AddListener(EventType.UpdateOxygenModifiersEvent, param => UpdateOxygenModifiers());
        this.AddListener(EventType.EntityDeathEvent, enemyType => _progression.AddXP((string) enemyType));
        _progression = new PlayerProgression(progressionData);
    }

    private void Start() {
        this.FireEvent(EventType.RefreshModifiersEvent);
    }

    private void Update() {
#if DEBUG
        DebugGUI.Instance.UpdateText(nameof(PlayerStats),
        "\nProgression\n" + 
            $"XP: {_progression.CurrentXp}\n" +
            $"XP cap: {_progression.CurrentXpCap}\n" +
            $"Level: {_progression.CurrentLevel}\n" +
            $"Skill points: {_progression.SkillPoints}\n" +
            $"\nSpec types\n" +
            $"Vigor: {_progression.CurrentSpec[PlayerProgression.SpecType.Vigor]}\n"+
            $"Endurance: {_progression.CurrentSpec[PlayerProgression.SpecType.Endurance]}\n" +
            $"Strength: {_progression.CurrentSpec[PlayerProgression.SpecType.Strength]}\n" +
            $"Dexterity: {_progression.CurrentSpec[PlayerProgression.SpecType.Dexterity]}\n"        
            );
#endif
    }

    private void UpdateCombatModifiers(MeleeSequenceData meleeData)
    {
        //Modifying in SO
        // foreach (var seq in meleeData.OrderToAttributes.Values) {
        //     seq.DmgScale = 1; //Default 1 haven't implemented damage scaling yet
        //     seq.DmgModifer = seq.DmgScale * level;
        //     seq.AtkSpdModifier = dexterity;
        // }
    }

    public void AddSkillType(PlayerProgression.SpecType type) {
        _progression.AddSkillType(type);
    }
}
