using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using DG.Tweening;
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

    public struct InventoryToggleMsg {
        public bool state;
    }

    public struct InventoryUpdateMsg {
        public float currentWeight;
        public float maxWeight;
        public bool itemOnly;
        public InventoryItem activeItem;
    }
    
    public class InventoryUI : SerializedMonoBehaviour {
        [SerializeField] private List<ItemUIData> itemList;
        [SerializeField] private List<ItemUIData> enabledItemList;
        [SerializeField] private ItemManager manager;
        [SerializeField] private ItemData defaultItem;
        private ItemUIData _activeItem;
        private ItemUIData _hoveredItem;
        private int _activeIndex;
        private int _hoveredIndex;

        [TitleGroup("Item list")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform itemParent;
        [SerializeField] private VerticalLayoutGroup itemLayout;
        private float layoutSpace;
        private float prefabHeight;

        [TitleGroup("Weight")]
        [SerializeField] private Slider weightSlider;
        [SerializeField] private TextMeshProUGUI weightText;
        
        [TitleGroup("Item details")]
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDesc;
        private const string nameFormat = "$ INFO <{0}>";
        private const string descFormat = "> {0}";

        [TitleGroup("Tween settings")] 
        [SerializeField] private float scrollDuration;


        private void Awake() {
            this.AddListener(EventType.ItemAddEvent, msg => UpdateItem((ItemMsg) msg));
            this.AddListener(EventType.ItemRemoveEvent, msg => UpdateItem((ItemMsg) msg));
            this.AddListener(EventType.InventoryToggleEvent, msg => ShowInventoryUI((InventoryToggleMsg) msg));
            this.AddListener(EventType.InventoryUpdateEvent, msg => UpdateInventoryUI((InventoryUpdateMsg) msg));
            
            foreach (var item in manager.itemList) {
                var obj = Instantiate(itemPrefab, itemParent).GetComponent<InventoryItemUI>();
                itemList.Add(new ItemUIData {
                    data = item,
                    uiRef = obj,
                    count = 1
                });
                obj.countText.text = "x00";
                obj.nameText.text = item.itemName.Substring(0, 4);
                obj.weightText.text = item.weight.ToString();
                itemParent.GetComponent<RectTransform>().SetAsFirstSibling();
                obj.gameObject.SetActive(false);
            }

            var defaultUIItem = itemList.Find(x => x.data == defaultItem);
            defaultUIItem.uiRef.gameObject.SetActive(true);
            _activeItem = defaultUIItem;
            enabledItemList.Add(defaultUIItem);
            
            _activeIndex = 0;
            layoutSpace = itemLayout.spacing;
            prefabHeight = itemPrefab.GetComponent<RectTransform>().rect.height;
            
            gameObject.SetActive(false);
        }

        private void Update() { 
            MoveItemList();
            if (Input.GetMouseButtonDown(0)) {
                ChooseItem();
                this.FireEvent(EventType.InventoryToggleEvent, new InventoryToggleMsg{state = false});
            }
        }

        private void UpdateItem(ItemMsg msg) {
            var item =  itemList.Find(x => x.data == msg.data);
            if (item.data == null) return;
            var ui = item.uiRef;
            item.count += msg.count;
            item.uiRef.gameObject.SetActive(item.count > 0);
            if (item.count > 0) enabledItemList.Add(itemList.Find(x => x.data == msg.data));
            ui.countText.text = $"x{item.count:D2}";
            enabledItemList = enabledItemList.OrderBy(x => {
                return itemList.IndexOf(itemList.Find(y => y.data == x.data));
            }).ToList();
        }

        private void ChooseItem() {
            this.FireEvent(EventType.ItemPickEvent, _hoveredItem.data);
            _activeItem = _hoveredItem;
            _activeIndex = _hoveredIndex;
        }

        private void ShowInventoryUI(InventoryToggleMsg msg) {
            _hoveredIndex = _activeIndex;
            _hoveredItem = _activeItem;
            UpdateInventoryUI(new InventoryUpdateMsg {
                activeItem = new InventoryItem(_activeItem.data),
                itemOnly = true
            });
            gameObject.SetActive(msg.state);
        }

        private void UpdateInventoryUI(InventoryUpdateMsg msg) {
            var data = msg.activeItem.data;
            itemName.text      = String.Format(nameFormat, data.itemName.Replace(" ", "_"));
            itemDesc.text      = String.Format(descFormat, data.itemDescription);
            if (msg.itemOnly) return;
            weightSlider.value = msg.currentWeight / msg.maxWeight;
            weightText.text    = $"{msg.currentWeight:0.0}/{msg.maxWeight:0.0}";
        }

        private void MoveItemList() {
            var scroll = Input.mouseScrollDelta;
            if (scroll.y == 0) return;
            var dist = layoutSpace + prefabHeight;
            var delta = scroll.y > 0 ? -1 : 1;
            if (enabledItemList.Count == 1) return;
            if (_hoveredIndex - delta < 0 || _hoveredIndex - delta > enabledItemList.Count - 1) return;
            _hoveredIndex            -= delta;
            _hoveredItem = enabledItemList[_hoveredIndex];
            itemParent.DOLocalMove(itemParent.transform.localPosition + new Vector3(0f, delta * dist, 0f), scrollDuration, true).SetEase(Ease.InOutExpo);
            UpdateInventoryUI(new InventoryUpdateMsg {
                activeItem = new InventoryItem(enabledItemList[_hoveredIndex].data),
                itemOnly = true
            });
        }
    }
}