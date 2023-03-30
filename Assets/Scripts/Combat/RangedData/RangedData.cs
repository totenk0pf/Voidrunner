using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "RangedData", menuName = "Player/RangedData", order = 0)]
    public class RangedData : ScriptableObject
    { 
        [SerializeField] private RangedAttribute rangedAttribute;

        public RangedAttribute Attribute => rangedAttribute;
    }
}