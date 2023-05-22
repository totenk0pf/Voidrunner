using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Enemy {
    public struct EnemyMovelist {
        public AnimParam anim;
        public float weight; 
    }
    
    [CreateAssetMenu(fileName = "EnemyMovelist", menuName = "Enemies/EnemyMovelist", order = 0)]
    public class EnemyMovelistData : SerializedScriptableObject { 
        public HardReferenceAnimData data;
        
        [ValueDropdown("GetAnimData", IsUniqueList = true, ExpandAllMenuItems = true, HideChildProperties = true)] [ShowInInspector]
        [SerializeField] private List<AnimParam> moves;
        
        public List<EnemyMovelist> moveList = new(); 
        
        protected IEnumerable GetAnimData(){
            if (!data) return new List<AnimParam>();
            return data.animParams.Select(x => new ValueDropdownItem(x.name, x));
        }

        [Button("Validate data")]
        private void ValidateData(){
            if (!data) return;
            moveList.Clear();
            foreach (var x in moves) {
                moveList.Add(new EnemyMovelist {
                    anim = x,
                    weight = 0,
                });
            }
        }
    }                                         
}