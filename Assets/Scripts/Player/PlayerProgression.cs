using System;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Level;
using Rooms;
using Sirenix.OdinInspector;
using UI;
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

    public class CheckpointData {
        public Dictionary<SkillType, SkillValue> skillValues = new();
        public int level;
        public float xp;
        public RoomObject roomToReset;
    }

    public class PlayerProgression : MonoBehaviour {
        [ReadOnly] public int level = 1;
        public int baseLevelUpXP = 50;
        public Dictionary<SkillType, SkillValue> skillValues = new() {
            { SkillType.Vigor , new SkillValue(1)},
            { SkillType.Endurance , new SkillValue(1)},
            { SkillType.Strength , new SkillValue(1)},
            { SkillType.Dexterity , new SkillValue(1)}
        };
        
        private int _currSkillPoints;
        private float _currentXP = 20f;
        private float _levelUpXP;

        private Oxygen _oxygen;
        private CheckpointData _currCPdata = new();
        private List<Door> _doorWalkedList = new();

        private void Awake() {
            //get save here
            _levelUpXP = level == 1 ? baseLevelUpXP : 50 * Mathf.Pow(1.2f, level);
            _oxygen = gameObject.GetComponent<Oxygen>();
        }

        private void Start() {
            _oxygen.oxygenPool = 100 + 5 * skillValues[SkillType.Vigor].level;
            
            this.AddListener(EventType.UpdateCombatModifiersEvent
                , param => UpdateCombatModifiers((MeleeSequenceData) param));
            
            EventDispatcher.Instance.AddListener(EventType.SetCheckpoint
                ,room => SetCheckpoint((RoomObject) room));
            
            EventDispatcher.Instance.AddListener(EventType.AddXP
                ,xp => AddXP((float) xp));
            
            EventDispatcher.Instance.AddListener(EventType.AddSkillLevel
                ,type => AddSkillLevel((SkillType) type));
            
            EventDispatcher.Instance.AddListener(EventType.SetCheckpoint
                ,room => SetCheckpoint((RoomObject) room));
            
            EventDispatcher.Instance.AddListener(EventType.OnPlayerEnterDoor
                ,door => HandlePlayerEnterDoor((Door) door));
            
            //Temp
            EventDispatcher.Instance.AddListener(EventType.OnPlayerDie
                ,_=>HandlePlayerDie());
            
            foreach (var skill in skillValues.Keys) {
                UpdatePlayerStat(skill);
            }
            
            FireUI();
        }
        
        //Events
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

        private void SetCheckpoint(RoomObject currentRoom) {
            _currCPdata.skillValues = skillValues;
            _currCPdata.level = level;
            _currCPdata.roomToReset = currentRoom;
            _currCPdata.xp = _currentXP;
        }

        private void HandlePlayerDie() {
            skillValues = _currCPdata.skillValues;
            _currentXP = _currCPdata.xp;
            level = _currCPdata.level;
            foreach (var door in _doorWalkedList) {
                door.ResetDoor(_currCPdata.roomToReset);
            }
            FireUI();
            _doorWalkedList.Clear();
            
            //temp
            var doorTransform = _currCPdata.roomToReset.gameObject.transform;
            gameObject.transform.position = doorTransform.position + new Vector3(0, doorTransform.localScale.y, 0);
            _oxygen.permanentOxygen = _oxygen.oxygenPool;
            _oxygen.FireUIEvent();
        }

        private void HandlePlayerEnterDoor(Door door) {
            if (_doorWalkedList.Contains(door)) return;
            _doorWalkedList.Add(door);
        }
        
        #region Main Methods

        private void AddXP(float amount) {
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
            FireUI();
        }

        private void AddSkillLevel(SkillType type) {
            if (_currSkillPoints <= 0 && skillValues[type].level >= skillValues[type].levelCap) return;
            skillValues[type].level++;
            UpdatePlayerStat(type);
        }
        #endregion

        #region Helper Function
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

        private void FireUI() {
            EventDispatcher.Instance.FireEvent(EventType.UITextChangedEvent, new TextUIObj {
                type  = TextUI.TextType.Experience,
                value = level
            });
        
            EventDispatcher.Instance.FireEvent(EventType.UIBarChangedEvent, new BarUIMsg {
                type  = BarUI.BarType.Experience,
                value = _currentXP / _levelUpXP,
            });
        }
        
        #endregion
    }
}
