using System;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "RangedData", menuName = "Player/RangedData", order = 0)]
    public class RangedData : ScriptableObject
    { 
        [SerializeField] private RangedAttribute rangedAttribute;

        public RangedAttribute Attribute => rangedAttribute;

        //Clear enemies list on play
        private void OnEnable() {
            ClearEnemiesCache();
        }

        public void ClearEnemiesCache() {
            Attribute.Enemies.Clear();
        }
    }
}