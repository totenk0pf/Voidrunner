using System;
using System.Collections.Generic;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player.AnimationSystem
{
    [CreateAssetMenu(fileName = "SerializedAnimationStateData", menuName = "Player/SerializedAnimationStateData", order = 0)]
    public class AnimationStateData : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<PlayerAnimState, AnimStateContainer> animationStates;
        
        public Dictionary<PlayerAnimState, AnimStateContainer> AnimationStates => animationStates;

        public AnimStateContainer GetAnimParam(PlayerAnimState state)
        {
            NCLogger.Log($"state: {state}");
            var container = animationStates[state];
            if (container != null && !string.IsNullOrEmpty(container.paramName)) return container;
            NCLogger.Log($"AnimParam doesn't exist", LogLevel.ERROR);
            return null;
        }
    }
}