using System;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using EventDispatcher = Core.Events.EventDispatcher;
using EventType = Core.Events.EventType;

namespace Player {
    [Serializable]
    public enum SkillType {
        Vigor,
        Endurance,
        Strength,
        Dexterity
    }
    
    [Serializable]
    public class SkillValue {
        public int level;
        public int levelCap;

        public SkillValue(int level, int levelCap = 50) {
            this.level = level;
            this.levelCap = levelCap;
        }
    }

    public class PlayerProgression : MonoBehaviour {
        [ReadOnly] public int level = 1;
        public int baseLevelUpXP = 50;
        public readonly Dictionary<SkillType, SkillValue> skillValues = new() {
            { SkillType.Vigor , new SkillValue(1)},
            { SkillType.Endurance , new SkillValue(1)},
            { SkillType.Strength , new SkillValue(1)},
            { SkillType.Dexterity , new SkillValue(1)}
        };
        
        private int _currSkillPoints;
        private float _currentXP;
        private float _levelUpXP;

        private Oxygen _oxygen;
        
        private void Awake() {
            //get save here
            _levelUpXP = level == 1 ? baseLevelUpXP : 50 * Mathf.Pow(1.2f, level);
            _oxygen = gameObject.GetComponent<Oxygen>();
        }

        private void Start() {
            this.AddListener(EventType.UpdateCombatModifiersEvent, param => UpdateCombatModifiers((MeleeSequenceData) param));
            foreach (var skill in skillValues.Keys) {
                UpdatePlayerStat(skill);
            }
        }

        private void UpdateCombatModifiers(MeleeSequenceData meleeData) {
            //Modifying in SO
            foreach (var seq in meleeData.OrderToAttributes.Values) {
                seq.DmgScale = 1; //Default 1 haven't implemented damage scaling yet
                seq.DmgModifer = seq.DmgScale * level;
                seq.AtkSpdModifier = skillValues[SkillType.Dexterity].level;
            }
        
            //Combat Modifiers
            //meleeBase.damageScale = 1; //Default 1 haven't implemented damage scaling yet
            //meleeBase.damageModifier = meleeBase.damageScale * level;
            //meleeBase.attackSpeedModifier = dexterity;
        }
        private float GetXpRatio => _currentXP / _levelUpXP;

        public void AddXP(float amount) {
            switch (_currentXP + amount >= _levelUpXP) {
                case true:
                    _currentXP = (_currentXP + amount) - _levelUpXP;
                    _levelUpXP = 50 * Mathf.Pow(1.2f, level);
                    level++;
                    _currSkillPoints++;
                    break;
                
                default:
                    _currentXP += amount;
                    break;
            }
            
            EventDispatcher.Instance.FireEvent(EventType.LevelChangeEvent, GetXpRatio);
        }

        public void AddSkillLevel(SkillType type) {
            if (_currSkillPoints <= 0 && skillValues[type].level >= skillValues[type].levelCap) return;
            skillValues[type].level++;
            UpdatePlayerStat(type);
        }

        private void UpdatePlayerStat(SkillType type) {
            switch (type) {
                case SkillType.Vigor:
                    _oxygen.oxygenPool = 100 + 5 * skillValues[type].level;
                    break;
                case SkillType.Endurance:
                    //set relevant comp here
                    break;
                case SkillType.Strength:
                    //set relevant comp here
                    break;
                case SkillType.Dexterity:
                    //set relevant comp here
                    break;
            }

        }
    }
}