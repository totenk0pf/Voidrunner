using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public struct AnimParam {
    public AnimatorControllerParameterType type;
    public string name;
    public int hash;
    
    [ShowIf("type", AnimatorControllerParameterType.Float)]
    public float floatParam;
    [ShowIf("type", AnimatorControllerParameterType.Int)]
    public int intParam;
    [ShowIf("type", AnimatorControllerParameterType.Bool)]
    public bool boolParam;
}


[Serializable]
public class AnimParamContainer
{
    [ReadOnly] public HardReferenceAnimData data;
    //public AnimatorControllerParameterType paramType;
    [ValueDropdown("GetParamType", IsUniqueList = true, ExpandAllMenuItems = true, HideChildProperties = true)] [ShowInInspector]
    public AnimParam param;
    
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
}
