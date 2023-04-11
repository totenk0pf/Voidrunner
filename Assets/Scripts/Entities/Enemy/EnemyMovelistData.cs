using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Enemy {
    [CreateAssetMenu(fileName = "EnemyMovelist", menuName = "Enemies/EnemyMovelist", order = 0)]
    public class EnemyMovelistData : SerializedScriptableObject { 
        public EnemyAnimData data;
        
        [Space]
        [ValueDropdown("GetAnimData", IsUniqueList = true, ExpandAllMenuItems = true, HideChildProperties = true)] [ShowInInspector]
        [SerializeField] private List<AnimParam> moves;
    
        [Space]
        public WeightedArray<AnimParam> weightedMoves = new WeightedArray<AnimParam>();

        protected IEnumerable GetAnimData(){
            if (!data) return new List<AnimParam>();
            return data.animParams.Select(x => new ValueDropdownItem(x.name, x));
        }

        [Button("Validate data")]
        private void ValidateData(){
            if (!data) return;
            weightedMoves.Clear();
            foreach (var x in moves) {
                weightedMoves.AddElement(x, 0);
            }
        }
    }                                         
}