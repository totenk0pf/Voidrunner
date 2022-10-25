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

        public SpawnData walker;
        public SpawnData crawler;
        public SpawnData juggernaut;
        
        //GameObject: Type of Enemy, float: Weight
        private Dictionary<GameObject, float> _enemyDictionary = new Dictionary<GameObject, float>();
        private WeightedArray<GameObject> _enemyWeightedList = new WeightedArray<GameObject>();

        private void Awake() {
            _enemyDictionary.Add(walker.prefab, walker.weight);
            _enemyDictionary.Add(crawler.prefab, crawler.weight);
            _enemyDictionary.Add(juggernaut.prefab, juggernaut.weight);

            foreach (var enemy in _enemyDictionary)
            {
                _enemyWeightedList.AddElement(enemy.Key, enemy.Value);
            }
        }

        private void Start() {
            var enemyToSpawn = _enemyWeightedList.GetRandomItem();
            Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        }
    }
}
