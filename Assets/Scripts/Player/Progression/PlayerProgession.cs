using System.Collections.Generic;
using UnityEngine;
using EventDispatcher = Core.Events.EventDispatcher;
using EventType = Core.Events.EventType;

public class PlayerProgession
{
    public enum SpecType
    {
        Vigor,
        Endurance,
        Strength,
        Dexterity
    }

    private int _currentLevel;
    private Dictionary<SpecType, int> _currentSpec;
    private int _specCap;
    
    private float _currentXP;
    private float _currentXpCap;

    private int _skillPoints;
    public int SkillPoints => _skillPoints;

    public void AddXP(float amount, float xpGainMod) {
        if (_currentXP + amount >= _currentXpCap) {
            _currentXP = _currentXP + amount - _currentXpCap;
            _currentXpCap = 50 * Mathf.Pow(xpGainMod, _currentLevel);
            _currentLevel++;
            _skillPoints++;
        } else {
            _currentXP += amount;
        }
        EventDispatcher.Instance.FireEvent(EventType.LevelUpEvent, GetXpRatio());
    }

    private void AddSkillType(SpecType type) {
        _currentSpec[type]++;
        EventDispatcher.Instance.FireEvent(EventType.SpecUpEvent, type);
    }

    public int GetSpecByType(SpecType type) => _currentSpec[type];
    private float GetXpRatio() => _currentXP / _currentXpCap;
}