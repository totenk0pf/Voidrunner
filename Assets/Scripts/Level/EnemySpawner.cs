using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Collections;

namespace Level
{
    public enum EnemyType {
        Walker,
        Crawler,
        Juggernaut
    }
    
    public class EnemySpawner : MonoBehaviour
    {
        [Serializable]
        public struct SpawnData {
            public EnemyType type;
            public GameObject prefab;
            public float weight;
        }

        //GameObject: Type of Enemy, float: Weight
        private List<SpawnData> _enemies = new();
        private WeightedArray<SpawnData> _enemyWeightedList = new();

        private void Awake() {
            foreach (var enemy in _enemies) {
                _enemyWeightedList.AddElement(enemy, enemy.weight);
            }
            Instantiate(_enemyWeightedList.GetRandomItem().prefab, transform.position, Quaternion.identity);
        }
    }
}
