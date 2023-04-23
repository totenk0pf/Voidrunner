using System;
using System.Collections.Generic;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player.AnimationSystem
{
    
    [CreateAssetMenu(fileName = "SerializedAnimationParamData", menuName = "Player/SerializedAnimationParamData", order = 0)]
    public class AnimationParamData : SerializedScriptableObject, IHardReferenceAnim
    {
        [SerializeField] private HardReferenceAnimData data;
        [SerializeField] private Dictionary<PlayerAnimState, AnimParamContainer> animationStates;

        public Dictionary<PlayerAnimState, AnimParamContainer> AnimationStates => animationStates;

        public AnimParamContainer GetAnimParam(PlayerAnimState state)
        {
            var container = animationStates[state];
            if (container != null && !string.IsNullOrEmpty(container.param.name)) return container;
            NCLogger.Log($"AnimParam doesn't exist", LogLevel.ERROR);
            return null;
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
                val.data = data;
            }
        }
    }
}