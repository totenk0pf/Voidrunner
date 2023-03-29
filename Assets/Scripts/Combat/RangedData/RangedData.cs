using UnityEngine;

namespace Combat.RangedData
{
    [CreateAssetMenu(fileName = "RangedData", menuName = "Player/RangedData", order = 0)]
    public class RangedData : ScriptableObject
    {
        [SerializeField] private float preshotDelay;
        [SerializeField] private float aftershotDelay;
        
    }
}