using System.Collections.Generic;
using Player.Progression;
using UI;
using UnityEngine;
using EventDispatcher = Core.Events.EventDispatcher;
using EventType = Core.Events.EventType;

public class PlayerProgression {
    private readonly PlayerProgressionData _progressionData;
    
    public PlayerProgression(PlayerProgressionData data) {
        _progressionData = data;
        CurrentSpec = new() {
            {SpecType.Vigor, 1},
            {SpecType.Endurance, 1},
            {SpecType.Strength, 1},
            {SpecType.Dexterity, 1}
        };
    }
    
    public enum SpecType {
        Vigor,
        Endurance,
        Strength,
        Dexterity
    }

    public int CurrentLevel { get; private set; }

    public Dictionary<SpecType, int> CurrentSpec { get; private set; }
    private int _specCap;
    
    public float CurrentXp { get; private set; }
    public float CurrentXpCap { get; private set; }
    
    public int SkillPoints { get; private set; }

    public void AddXP(float amount) {
        if (CurrentXp + amount >= CurrentXpCap) {
            CurrentXp = CurrentXp + amount - CurrentXpCap;
            CurrentXpCap = 50 * Mathf.Pow(_progressionData.XPGainMod, CurrentLevel);
            CurrentLevel++;
            SkillPoints++;
            EventDispatcher.Instance.FireEvent(EventType.LevelUpEvent, CurrentLevel);
        } else {
            CurrentXp += amount;
        }
        EventDispatcher.Instance.FireEvent(EventType.XPGainEvent, amount);
        EventDispatcher.Instance.FireEvent(EventType.UIBarChangedEvent, new BarUIMsg {
            type = BarUI.BarType.Experience,
            value = GetXpRatio()
        });
    }
    
    public void AddXP(string enemyType) {
        var amt = _progressionData.enemyExperienceData.Find(x => x.tag == enemyType).gain;
        if (CurrentXp + amt >= CurrentXpCap) {
            CurrentXp    = CurrentXp + amt - CurrentXpCap;
            CurrentXpCap = 50 * Mathf.Pow(_progressionData.XPGainMod, CurrentLevel);
            CurrentLevel++;
            SkillPoints++;
            EventDispatcher.Instance.FireEvent(EventType.LevelUpEvent, CurrentLevel);
        } else {
            CurrentXp += amt;
        }
        EventDispatcher.Instance.FireEvent(EventType.XPGainEvent, amt);
        EventDispatcher.Instance.FireEvent(EventType.UIBarChangedEvent, new BarUIMsg {
            type  = BarUI.BarType.Experience,
            value = GetXpRatio()
        });
    }

    private void AddSkillType(SpecType type) {
        var specAmt = GetSpecByType(type);
        specAmt++;
        EventDispatcher.Instance.FireEvent(EventType.SpecUpEvent, type);
        switch (type) {
            case SpecType.Vigor:
                EventDispatcher.Instance.FireEvent(EventType.UpdateOxygenData, specAmt);
                break;
            case SpecType.Endurance:
                EventDispatcher.Instance.FireEvent(EventType.UpdateInventoryData, specAmt);
                break;
            case SpecType.Strength:
            case SpecType.Dexterity:
                EventDispatcher.Instance.FireEvent(EventType.UpdateCombatData, specAmt);
                break;
        }
    }

    public int GetSpecByType(SpecType type) => CurrentSpec[type];
    private float GetXpRatio() => CurrentXp / CurrentXpCap;
}