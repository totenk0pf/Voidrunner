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
    }
}