using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Patterns;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Enemy {
    [CreateAssetMenu(fileName = "SerializedAnimData", menuName = "Enemies/SerializedAnimData", order = 0)]
    public class AnimSerializedData : SerializedScriptableObject, IHardReferenceAnim {
        public HardReferenceAnimData data;
        
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
        
        [Button("Validate & Create Refs")]
        public void ValidateData()
        {
            //Validate hard reference data
            data.ValidateData();
        }
    }
}