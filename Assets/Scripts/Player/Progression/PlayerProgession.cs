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
    }
    
    public enum SpecType
    {
        Vigor,
        Endurance,
        Strength,
        Dexterity
    }

    private int _currentLevel;
    public int CurrentLevel => _currentLevel;
    private Dictionary<SpecType, int> _currentSpec = new Dictionary<SpecType, int>() {
        {SpecType.Vigor, 1},
        {SpecType.Endurance, 1},
        {SpecType.Strength, 1},
        {SpecType.Dexterity, 1}
    };
    private int _specCap;
    
    private float _currentXP;
    private float _currentXpCap;

    private int _skillPoints;
    public int SkillPoints => _skillPoints;

    public void AddXP(float amount) {
        if (_currentXP + amount >= _currentXpCap) {
            _currentXP = _currentXP + amount - _currentXpCap;
            _currentXpCap = 50 * Mathf.Pow(_progressionData.XPGainMod, _currentLevel);
            _currentLevel++;
            _skillPoints++;
            EventDispatcher.Instance.FireEvent(EventType.LevelUpEvent, _currentLevel);
        } else {
            _currentXP += amount;
        }
        EventDispatcher.Instance.FireEvent(EventType.XPGainEvent, amount);
        EventDispatcher.Instance.FireEvent(EventType.UIBarChangedEvent, new BarUIMsg {
            type = BarUI.BarType.Experience,
            value = GetXpRatio()
        });
    }
    
    public void AddXP(string enemyType) {
        var amt = _progressionData.enemyExperienceData.Find(x => x.tag == enemyType).gain;
        if (_currentXP + amt >= _currentXpCap) {
            _currentXP    = _currentXP + amt - _currentXpCap;
            _currentXpCap = 50 * Mathf.Pow(_progressionData.XPGainMod, _currentLevel);
            _currentLevel++;
            _skillPoints++;
            EventDispatcher.Instance.FireEvent(EventType.LevelUpEvent, _currentLevel);
        } else {
            _currentXP += amt;
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

    public int GetSpecByType(SpecType type) => _currentSpec[type];
    private float GetXpRatio() => _currentXP / _currentXpCap;
}