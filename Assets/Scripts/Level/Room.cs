using System;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

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
        [ReadOnly] public List<Tuple<Vector2Int, Quaternion>> Rotations { get; private set; }
        [SerializeField] private BoxCollider col;
        [SerializeField] private List<EntryInfo> entries;
        [SerializeField] private RoomType type;
        [ReadOnly] [ShowInInspector] private Vector3 _pivot;
        private Bounds _worldSpaceBounds;

        [HorizontalGroup("Generate")]
        [Button("Generate bounds")]
        private void GenerateBounds() {
            if (!col) {
                NCLogger.Log("Collider component not found!", LogLevel.ERROR);
                return;
            }

            var tempBounds     = new Bounds();
            var childColliders = transform.GetComponentsInChildren<Collider>();
            for (int i = 0; i < childColliders.Length; i++) {
                if (childColliders[i] == col) continue;
                if (i == 1) tempBounds = childColliders[i].bounds;
                tempBounds.Encapsulate(childColliders[i].bounds);
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
        [Button("Generate rotations")]
        private void GenerateRotation() {
            foreach (var entry in entries) {
                var normal          = entry.offset.normalized;
                var currentRotation = transform.rotation;
                for (int i = 1; i < 4; i++) {
                    var rotatedNormal = Quaternion.Euler(0, 90 * i, 0) * normal;
                    
                }
            }
        }

        [Button("Validate")]
        private void Validate() {
            GenerateBounds();
            GenerateEntries();
        }

        private void OnEnable() {
            Validate();
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (!col) {
                if (TryGetComponent(out BoxCollider component)) col = component;
            }
        }

        private void OnDrawGizmos() {
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