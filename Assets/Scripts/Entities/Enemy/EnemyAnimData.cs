using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor.Animations;
using UnityEngine;

namespace Entities.Enemy {
    [Serializable]
    public struct AnimParam {
        public AnimatorControllerParameterType type;
        public string name;
        public int hash;
    }
    
    [CreateAssetMenu(fileName = "EnemyAnimData", menuName = "Enemies/AnimData", order = 0)]
    public class EnemyAnimData : SerializedScriptableObject {
        [ReadOnly] [ShowInInspector] public List<AnimParam> animParams = new();
        [SerializeField] private AnimatorController controller;

        [Button("Validate data")]
        private void ValidateData(){
            if (!controller) return;
            animParams.Clear();
            foreach (var x in controller.parameters) {
                animParams.Add(new AnimParam {
                   type = x.type,
                   name = x.name,
                   hash = x.nameHash
                });
            }
        }
    }
}