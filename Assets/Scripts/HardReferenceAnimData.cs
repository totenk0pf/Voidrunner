using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

[Serializable]
public struct AnimParam {
    public AnimatorControllerParameterType type;
    [ReadOnly] public string name;
    [ReadOnly] public int hash;
}


[Serializable]
public class AnimParamContainer
{
    [ReadOnly] public HardReferenceAnimData data;
    //public AnimatorControllerParameterType paramType;
    [ValueDropdown("GetParamType", IsUniqueList = true, ExpandAllMenuItems = true, HideChildProperties = true)] [ShowInInspector]
    public AnimParam param;

    [ShowIf("Type", AnimatorControllerParameterType.Float)]
    public float floatParam;
    [ShowIf("Type", AnimatorControllerParameterType.Int)]
    public int intParam;
    [ShowIf("Type", AnimatorControllerParameterType.Bool)]
    public bool boolParam;
    
    public int Hash => param.hash;
    public AnimatorControllerParameterType Type => param.type;
    public string Name => param.name;
    
    private IEnumerable GetParamType(){
        if (!data) return new List<AnimParam>();
        return data.animParams.Select(x => new ValueDropdownItem(x.name, x));
    }
}

[CreateAssetMenu(fileName = "HardReferenceAnimData", menuName = "Asset/HardReferenceAnimData", order = 0)]
public class HardReferenceAnimData : SerializedScriptableObject, IHardReferenceAnim {
    [ReadOnly] [ShowInInspector] public List<AnimParam> animParams = new();
#if UNITY_EDITOR
    [SerializeField] private AnimatorController controller;

    [Button("Validate data")]
    public void ValidateData(){
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
#endif
}
