using System;
using Core.Events;
using TMPro;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace UI {
    public class SkillPointUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI pointText;
        
        private void Awake() {
            SetChildrenState(false);
            EventDispatcher.Instance.AddListener(
                EventType.SkillPointGainedEvent, 
                point => UpdatePoints((int) point)
            );
        }
        private void UpdatePoints(int newPoints) {
            SetChildrenState(newPoints > 0);
            pointText.text = newPoints.ToString("D2");
        }

        private void SetChildrenState(bool state) {
            foreach (Transform i in transform) i.gameObject.SetActive(state);
        }
    }
}