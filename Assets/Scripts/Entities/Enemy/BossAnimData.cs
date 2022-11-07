using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Patterns;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Enemy {
    [CreateAssetMenu(fileName = "BossAnimData", menuName = "Enemies/BossAnimData", order = 0)]
    public class BossAnimData : SerializedScriptableObject {
        public EnemyAnimData data;
        
        [ValueDropdown("GetAnimData", IsUniqueList = true, ExpandAllMenuItems = true, HideChildProperties = true)] [ShowInInspector]
        public AnimParam idleAnim;
        
        [ValueDropdown("GetAnimData", IsUniqueList = true, ExpandAllMenuItems = true, HideChildProperties = true)] [ShowInInspector]
        public AnimParam hostileAnim;
        
        [ValueDropdown("GetAnimData", IsUniqueList = true, ExpandAllMenuItems = true, HideChildProperties = true)] [ShowInInspector]
        public List<AnimParam> attackAnim;
        
        protected IEnumerable GetAnimData(){
            if (!data) return new List<AnimParam>();
            return data.animParams.Select(x => new ValueDropdownItem(x.name, x));
        }
    }
}