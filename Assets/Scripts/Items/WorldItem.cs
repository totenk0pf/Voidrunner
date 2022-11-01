using System;
using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Items {
    public class WorldItem : MonoBehaviour, IItems {
        public ItemData itemRef;
        public int itemCount = 1;
        [SerializeField] private MeshFilter filter;
        [SerializeField] private MeshRenderer renderer;

        public virtual void OnPickup() {
            this.FireEvent(EventType.OnItemAdd, new PickupMsg {
                data = itemRef,
                count = itemCount
            });
            Destroy(gameObject);
        }

        protected void OnTriggerEnter(Collider col) {
            OnPickup();
        }

        private void OnValidate() {
            if (!itemRef) return;
            if (!filter) return;
            filter.sharedMesh = itemRef.model;
            renderer.material = itemRef.material;
        }
    }
}