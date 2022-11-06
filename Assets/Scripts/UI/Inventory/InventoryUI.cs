using System;
using System.Collections.Generic;
using Core.Events;
using Items;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace UI {
    [Serializable]
    public struct ItemUIData {
        public ItemData data;
        public InventoryItemUI uiRef;
        public int count;
    }
    
    public class InventoryUI : SerializedMonoBehaviour {
        [SerializeField] private List<ItemUIData> itemList;

        [TitleGroup("Item list")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform itemParent;

        [TitleGroup("Weight")]
        [SerializeField] private Slider weightSlider;
        [SerializeField] private TextMeshProUGUI weightText;
        
        [TitleGroup("Item details")]
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDesc;

        private void Awake() {
            this.AddListener(EventType.OnItemAdd, msg => UpdateItem((ItemMsg) msg));
            this.AddListener(EventType.OnItemRemove, msg => UpdateItem((ItemMsg) msg));
            foreach (var item in ItemManager.Instance.itemList) {
                var obj = Instantiate(itemPrefab, itemParent).GetComponent<InventoryItemUI>();
                itemList.Add(new ItemUIData {
                    data = item,
                    uiRef = obj,
                    count = 0
                });
                obj.countText.text = "x00";
                obj.nameText.text = item.itemName.Substring(0, 4);
                obj.weightText.text = item.weight.ToString();
                obj.gameObject.SetActive(false);
            }
        }

        private void UpdateItem(ItemMsg msg) {
            var item =  itemList.Find(x => x.data == msg.data);
            if (item.data == null) return;
            var ui = item.uiRef;
            item.count += msg.count;
            ui.countText.text = $"x{item.count:D2}";
        }
    }
}