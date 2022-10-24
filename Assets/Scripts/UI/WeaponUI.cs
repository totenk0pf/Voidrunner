using System;
using System.Collections.Generic;
using Combat;
using Core.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace UI {
    public class WeaponUI : MonoBehaviour {
        [Serializable]
        private struct UIEntry {
            public WeaponType type;
            public RectTransform element;
        }

        [SerializeField] private List<UIEntry> panelList = new();
        public WeaponType currentType;
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
            EventDispatcher.Instance.AddListener(EventType.WeaponChangedEvent, type => ChangeActivePanel((WeaponType) type));
        }

        private void ChangeActivePanel(WeaponType type) {
            currentType = type;
            _initHeight = Transform.rect.height - Layout.spacing;
            var segmentHeight = _initHeight / panelList.Count;
            foreach (var item in panelList) {
                var selectedScale = segmentHeight * activeScale / _initHeight;
                var unselectedScale = segmentHeight * inactiveScale / _initHeight;
                item.element.DOScale(item.type == currentType ? 
                                         new Vector3(selectedScale, selectedScale, 1) : 
                                         new Vector3(unselectedScale, unselectedScale, 1), 
                                     transitionTime)
                    .SetEase(Ease.InOutCubic)
                    .OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(Transform));
            }
        }
    }
}
