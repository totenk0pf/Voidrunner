using Audio;
using Core.Events;
using Unity.Mathematics;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Items {
    public class WorldItem : MonoBehaviour, IItems {
        public ItemData itemRef;
        public AudioClip pickupSound;
        [SerializeField] private MeshFilter filter;
        [SerializeField] private MeshRenderer renderer;
        [SerializeField] private GameObject fxPrefab;

        public virtual void OnPickup() {
            this.FireEvent(EventType.ItemAddEvent, new ItemMsg {
                data = itemRef
            });
            if (fxPrefab) Instantiate(fxPrefab, transform.position, Quaternion.identity, null);
            AudioManager.Instance.PlayClip(transform.position, pickupSound);
            Destroy(gameObject);
        }

        protected void OnTriggerEnter(Collider col) {
            OnPickup();
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (!itemRef) return;
            if (!filter) return;
            filter.sharedMesh = itemRef.model;
            renderer.material = itemRef.material;
        }
#endif
    }
}