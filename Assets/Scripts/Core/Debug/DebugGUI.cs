using System.Collections.Generic;
using System.Linq;
using Core.Patterns;
using TMPro;
using UnityEngine;

namespace Core.Debug {
    public class DebugGUI : Singleton<DebugGUI> {
        [SerializeField] private TextMeshProUGUI debugText;
        private Dictionary<string, string> _debugItems = new();

        public void UpdateText(string id, string text) {
            _debugItems[id] = text;
        }
#if UNITY_EDITOR
        private void Update() {
            debugText.text = string.Concat(_debugItems.Values.ToList());
        }
#endif
    }
}