using System;
using System.Collections.Generic;
using Core.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace UI {
    public struct BarUIMsg {
        public BarUI.BarType type;
        public float value;
    }
    
    public class BarUI : MonoBehaviour {
        public enum BarType {
            Oxygen,
            Experience
        }
        
        [Serializable]
        private struct BarItem {
            public Slider slider;
            public BarType type;
        }

        [SerializeField] private float lerpDuration;
        [SerializeField] private List<BarItem> barItems;

        private void Awake() {
            EventDispatcher.Instance.AddListener(EventType.UIBarChangedEvent, msg => UpdateBar((BarUIMsg) msg));
        }

        private void UpdateBar(BarUIMsg msg) {
            var slider = barItems.Find(x => x.type == msg.type).slider;
            if (!slider) return;
            DOTween.To(() => slider.value, x => slider.value = x, msg.value, lerpDuration).SetEase(Ease.InOutExpo);
        }
    }
}