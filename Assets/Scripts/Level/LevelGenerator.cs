using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Level {
    [ExecuteAlways]
    public class LevelGenerator : MonoBehaviour {
        [SerializeField] private Transform[] startDirection;
        [SerializeField] private int roomsPerBranch;
        [SerializeField] private GameObject hallwaySegment;
        [SerializeField] private List<Room> roomPrefabs;
        private List<Room> _generatedRooms;
        
        private bool _canGenerate = true;

        [DisableIf("_canGenerate", false)]
        [Button("Generate rooms")]
        private void Btn1() {
            StartCoroutine(GenerateRoom());
        }

        [Button("Cancel generation")]
        private void Btn2() {
            _canGenerate = true;
            StopCoroutine(GenerateRoom());
        }

        private void Awake() {
            // GenerateRooms();
            // GenerateHallways();
        }

        private IEnumerator GenerateRoom() {
            Random rand = new Random();
            yield return null;
        }

        private IEnumerator GenerateHallways() {
            
            yield return null;
        }

        private IEnumerator GenerateLevel() {
            _canGenerate = false;
            foreach (var item in startDirection) {
                var roomCount = 0;
                do {
                    yield return GenerateHallways();
                    yield return GenerateRoom();
                    roomCount++;
                } while (roomCount < roomsPerBranch);
            }
            yield return null;
        }
    }
}