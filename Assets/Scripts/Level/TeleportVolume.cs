using UnityEngine;
using StaticClass;
using UnityEditor;

namespace Level {
    [RequireComponent(typeof(BoxCollider))]
    public class TeleportVolume : MonoBehaviour {
        [SerializeField] private Vector3 relativeDestination;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private ParticleSystem teleportFX;

        [Header("Debug")]
        [SerializeField] private Vector3 labelOffset;
        [SerializeField] private float destinationRadius;

        private void Awake() {
            if (teleportFX) teleportFX.transform.localPosition = relativeDestination;
        }

        private void OnTriggerEnter(Collider other) {
            if (!CheckLayerMask.IsInLayerMask(other.gameObject, playerMask)) return;
            other.transform.position = transform.position + relativeDestination;
            if (teleportFX) teleportFX.Play();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Handles.color = Color.white;
            Vector3 target = transform.position + relativeDestination;
            Handles.Label(target + labelOffset, $"Destination\nPos: {relativeDestination}");
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target, destinationRadius);
            Gizmos.DrawLine(transform.position, target);
        }
#endif
    }
}