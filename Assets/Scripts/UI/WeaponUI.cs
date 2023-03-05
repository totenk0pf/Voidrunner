using System;
using System.Collections.Generic;
using Combat;
using Core.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
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
        private float _initHeight;

        [TitleGroup("Tween settings")] 
        [SerializeField] private Ease easeType;
        [SerializeField] private float transitionTime;
        
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
<<<<<<< HEAD
                             entry => ChangeActivePanel((WeaponManager.WeaponEntry) entry));
=======
                             entry => ChangeActivePanel((WeaponEntry) entry));
>>>>>>> c9f1fe8cad10044d48d8fb74e790012081e956ad
            this.AddListener(EventType.RangedShotEvent, data => UpdateRangedUI((RangedUIMsg) data));
            this.AddListener(EventType.WeaponFiredEvent, data => UpdateChargeUI((WeaponFireUIMsg) data));
        }

<<<<<<< HEAD
        private void ChangeActivePanel(WeaponManager.WeaponEntry entry) {
            currentType = entry.Type;
=======
        private void ChangeActivePanel(WeaponEntry entry) {
            currentType = entry.type;
>>>>>>> c9f1fe8cad10044d48d8fb74e790012081e956ad
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
                    .SetEase(easeType)
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
