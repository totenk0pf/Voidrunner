using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI {
    [Serializable]
    public class InventoryItemUI : MonoBehaviour {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI weightText;
        public TextMeshProUGUI countText;
        public Image itemSprite;

        public void UpdateUI(ItemData data) {
            nameText.text     = data.name.Substring(0, 4);
            weightText.text   = data.weight.ToString();
            itemSprite.sprite = data.icon;
        }
        
        public void UpdateUI(ItemData data, int count = 0) {
            nameText.text     = data.name.Substring(0, 4);
            weightText.text   = data.weight.ToString();
            countText.text    = $"x{count:D2}";
            itemSprite.sprite = data.icon;
        }

        public void UpdateUI(int count = 0) {
            countText.text = $"x{count:D2}";
        }
    }
}