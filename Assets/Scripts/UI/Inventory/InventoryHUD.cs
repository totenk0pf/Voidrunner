using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace UI {
    public struct InventoryHUDMsg {
        public ItemData data;
        public int count;
        public bool countOnly;
    }
    
    [RequireComponent(typeof(InventoryItemUI))]
    public class InventoryHUD : MonoBehaviour {
        private InventoryItemUI _itemUi;
        private InventoryItemUI itemUi {
            get {
                if (!_itemUi) _itemUi = GetComponent<InventoryItemUI>();
                return _itemUi;
            }
        }

        private void Awake() {
            this.AddListener(EventType.InventoryHUDEvent, msg => OnUpdate((InventoryHUDMsg) msg));
        }

        private void OnDestroy() {
            this.RemoveListener(EventType.InventoryHUDEvent, msg => OnUpdate((InventoryHUDMsg) msg));
        }

        private void OnUpdate(InventoryHUDMsg msg) {
            if (msg.countOnly) {
                itemUi.UpdateUI(msg.count);
            } else {
                itemUi.UpdateUI(msg.data, msg.count);
            }
        }
    }
}