using System;
using Core.Events;
using TMPro;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace UI {
    public class LevelUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI levelText;

        private void Awake() {
            this.AddListener(EventType.LevelUpEvent, level => UpdateText((int) level));
        }

        private void OnDestroy() {
            this.RemoveListener(EventType.LevelUpEvent, level => UpdateText((int) level));
        }

        private void UpdateText(int level) {
            levelText.text = level.ToString("D2");
        }
    }
}