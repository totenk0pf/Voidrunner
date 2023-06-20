using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using MagicLightProbes;
using EventType = UnityEngine.EventType;

namespace Level {
    public enum RoomType {
        Hub,
        Normal,
        Hallway,
        Boss,
        End
    }

    [Serializable]
    public class EntryInfo {
        public Transform transform;
        public Vector3 offset;
        public bool isLocked;
    }

    [RequireComponent(typeof(BoxCollider))]
    public class Room : MonoBehaviour {
        public RoomType type;
        public bool IsInRoom { get; private set; }
        [ReadOnly] public List<EnemyBase> enemiesInRoom;

        [ReadOnly] public List<Tuple<Vector2Int, Quaternion>> Rotations { get; private set; }
        [SerializeField] private BoxCollider col;
        [SerializeField] private List<EntryInfo> entries;
        [ReadOnly] [ShowInInspector] private Vector3 _pivot;
        private Bounds _worldSpaceBounds;

        [HorizontalGroup("Generate")]
        [Button("Generate bounds")]
        private void GenerateBounds() {
            if (!col) {
                NCLogger.Log("Collider component not found!", LogLevel.ERROR);
                return;
            }

            var hasBounds = false;
            var tempBounds     = new Bounds();

            var renderers = GetComponentsInChildren<Renderer>();
            
            for (var i = 0; i < renderers.Length; ++i) {
                Renderer childRenderer = renderers[i];
                if (childRenderer == null) continue;
                if (hasBounds) {
                    tempBounds.Encapsulate(childRenderer.bounds);
                } else {
                    tempBounds = childRenderer.bounds;
                    hasBounds  = true;
                }
            }

            _worldSpaceBounds = tempBounds;
            col.size          = tempBounds.size;
            col.center        = transform.worldToLocalMatrix.MultiplyPoint3x4(tempBounds.center);
            _pivot            = new Vector3(tempBounds.center.x, tempBounds.min.y, tempBounds.center.z);
            NCLogger.Log($"Bound generated: size of {col.size.ToString()}");
            NCLogger.Log($"Pivot generated: {_pivot.ToString()}");
        }

        [HorizontalGroup("Generate")]
        [Button("Generate entries")]
        private void GenerateEntries() {
            if (entries.Count < 1) return;
            foreach (var entry in entries) {
                var offset = entry.transform.position - _worldSpaceBounds.center;
                offset.y     = 0;
                entry.offset = offset;
            }
        }
        
        [HorizontalGroup("Generate")]
        [Button("Generate probes")]
        private void GenerateProbes() {
            
        }

        [Button("Validate")]
        private void Validate() {
            GenerateBounds();
            GenerateEntries();
        }

        private void OnEnable() {
            Validate();
        }

        private void Awake() {
            enemiesInRoom = GetComponentsInChildren<EnemyBase>().ToList();
            EventDispatcher.Instance.AddListener(Core.Events.EventType.OnEnemyDie, enemy => HandleEnemyDie((EnemyBase) enemy));
        }

        private void HandleEnemyDie(EnemyBase enemy) {
            if (enemiesInRoom.Contains(enemy)) enemiesInRoom.Remove(enemy);
        }

        [Button("Rotate collider")]
        private void RotateCollider() {
            var currentSize = col.size;
            var rotated = new Vector3(currentSize.z, currentSize.y, currentSize.x);
            col.size = rotated;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
                IsInRoom = true;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                Debug.Log($"[ROOM] Enemy in {transform.gameObject.name}");
            }

        }
        private void OnTriggerExit(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            IsInRoom = false;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (!col) {
                if (TryGetComponent(out BoxCollider component)) col = component;
            }
        }

        private void OnDrawGizmosSelected() {
            if (!Application.isPlaying) {
                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }

            if (!col) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            var labelPos = _pivot + Vector3.up * 2f;
            if (IsSceneViewCameraInRange(labelPos, 25f)) {
                Handles.Label(
                    labelPos, $"{transform.name}\nSize: {col.bounds.size.ToString()}\nPivot: {_pivot.ToString()}");
            }

            Gizmos.DrawWireSphere(_pivot, 0.5f);
            if (entries.Count < 1) return;
            foreach (var entry in entries) {
                if (!entry.transform) return;
                var worldSpacePos = transform.localToWorldMatrix.MultiplyPoint3x4(col.center + entry.offset);
                var pos           = new Vector3(worldSpacePos.x, _worldSpaceBounds.center.y, worldSpacePos.z);
                Handles.color = Handles.zAxisColor;
                Handles.ArrowHandleCap(0, pos, entry.transform.rotation, 1f, EventType.Repaint);
            }
        }

        public static bool IsSceneViewCameraInRange(Vector3 position, float distance) {
            Vector3 cameraPos = Camera.current.WorldToScreenPoint(position);
            return ((cameraPos.x >= 0) &&
                    (cameraPos.x <= Camera.current.pixelWidth) &&
                    (cameraPos.y >= 0) &&
                    (cameraPos.y <= Camera.current.pixelHeight) &&
                    (cameraPos.z > 0) &&
                    (cameraPos.z < distance));
        }

#endif
    }
}