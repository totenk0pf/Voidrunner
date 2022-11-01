using System;
using Core.Events;
using UnityEngine;

namespace Items {
    public class WorldItem : MonoBehaviour, IItems {
        public ItemData itemRef;
        public int itemCount;
        [SerializeField] private MeshFilter filter;
        [SerializeField] private MeshRenderer renderer;

        public virtual void OnPickup(InventorySystem inventory) {
            inventory.Add(itemRef, itemCount);
        }

        protected void OnTriggerEnter(Collider col) {
            var x = col.transform.GetComponent<InventorySystem>();
            if (!x) return;
            OnPickup(x);
            EventDispatcher.Instance.FireEvent(Core.Events.EventType.OnItemAdd, itemRef);
        }

        private void OnValidate() {
            if (!itemRef) return;
            if (!filter) return;
            filter.sharedMesh = itemRef.model;
            renderer.material = itemRef.material;
        }
    }
}