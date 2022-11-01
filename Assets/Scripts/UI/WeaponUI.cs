using System;
using System.Collections.Generic;
using Combat;
using Core.Events;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;


namespace UI {
    [Serializable]
    public struct UIEntry {
        public WeaponType type;
        public RectTransform element;
        public Slider rechargeSlider;
    }

    public struct RangedUIMsg {
        public int ammo;
        public int clip;
    }
    
    public struct WeaponFireUIMsg {
        public WeaponType type;
        public float rechargeDuration;
    }

    public class WeaponUI : MonoBehaviour {
        [SerializeField] private List<UIEntry> panelList = new();
        [SerializeField] private TextMeshProUGUI ammoText;
        private WeaponType currentType;
        [SerializeField] private float activeScale;
        [SerializeField] private float inactiveScale;
        [SerializeField] private float transitionTime;
        private float _initHeight;
        
        private VerticalLayoutGroup _layout;
        private VerticalLayoutGroup Layout {
            get {
                if (!_layout) _layout = GetComponent<VerticalLayoutGroup>();
                return _layout;
            }
        }
        private RectTransform _transform;
        private RectTransform Transform {
            get {
                if (!_transform) _transform = GetComponent<RectTransform>();
                return _transform;
            }
        }

        private void Awake() {
            this.AddListener(EventType.WeaponChangedEvent, 
                             entry => ChangeActivePanel((WeaponEntry) entry));
            this.AddListener(EventType.RangedShotEvent, data => UpdateRangedUI((RangedUIMsg) data));
            this.AddListener(EventType.WeaponFiredEvent, data => UpdateChargeUI((WeaponFireUIMsg) data));
        }

        private void ChangeActivePanel(WeaponEntry entry) {
            currentType = entry.type;
            _initHeight = Transform.rect.height - Layout.spacing;
            var segmentHeight = _initHeight / (panelList.Count);
            foreach (var item in panelList) {
                bool isCurrent = item.type == currentType;
                var selectedScale = segmentHeight * activeScale / _initHeight;
                var unselectedScale = segmentHeight * inactiveScale / _initHeight;
                item.element.DOScale(isCurrent ? 
                                         new Vector3(selectedScale, selectedScale, 1) : 
                                         new Vector3(unselectedScale, unselectedScale, 1), 
                                     transitionTime)
                    .SetEase(Ease.InOutCubic)
                    .OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(Transform));
                item.rechargeSlider.gameObject.SetActive(isCurrent);
            }
        }

        private void UpdateRangedUI(RangedUIMsg data) {
            ammoText.text = $"{data.ammo}/{data.clip}";
        }

        private void UpdateChargeUI(WeaponFireUIMsg data) {
            var slider = panelList.Find(x => x.type == data.type).rechargeSlider;
            slider.value = 0;
            var anim = DOTween.To(
                                  () => slider.value, 
                                  x => slider.value = x, 
                                  1f, 
                                  data.rechargeDuration).SetEase(Ease.Linear);
        }
    }
}
