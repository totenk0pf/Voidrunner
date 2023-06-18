using System;
using System.Collections.Generic;
using System.Linq;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player.AnimationSystem
{
    
    [CreateAssetMenu(fileName = "SerializedAnimationParamData", menuName = "Player/SerializedAnimationParamData", order = 0)]
    public class AnimationParamData : SerializedScriptableObject, IHardReferenceAnim
    {
        [SerializeField] private HardReferenceAnimData data;
        [SerializeField] private Dictionary<PlayerAnimState, List<AnimParamContainer>> animationStates;

        public Dictionary<PlayerAnimState, List<AnimParamContainer>> AnimationStates => animationStates;

        public List<AnimParamContainer> GetAnimParam(PlayerAnimState state)
        {
            var container = animationStates[state];

            if (container.Count == 0 || container.Any(param => string.IsNullOrEmpty(param.param.name))) {
                NCLogger.Log($"AnimParam doesn't exist", LogLevel.ERROR);
                return null;
            }
            
            return container;
        }

        private void OnEnable() {
            ValidateData();
        }

        [Button("Validate & Create Refs")]
        public void ValidateData()
        {
            //Validate hard reference data first
            data.ValidateData();
            //Populate ref later
            foreach (var val in animationStates.Values) {
                foreach (var container in val) {
                    container.data = data;
                }
            }
        }
    }
}