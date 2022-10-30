using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Core.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace UI {
    public class AugmentUI : MonoBehaviour {
        private EmpowerType _currentType;
        
        [TitleGroup("Data")]
        [SerializeField] private UIData uiData;
        [SerializeField] private Slider slider;
        private AugmentUIData _augmentData;
        private AugmentUIData _noneData;

        [TitleGroup("UI components")]
        [SerializeField] private Image augmentSprite;
        [SerializeField] private Image augmentBackground;
        [SerializeField] private List<Image> uiRenderers;

        private void Awake() {
            _noneData = uiData.GetByType(EmpowerType.None);
            _augmentData = uiData.GetByType(_currentType);
            EventDispatcher.Instance.AddListener(EventType.AugmentChangedEvent, type => OnElementChange((EmpowerType) type));
            EventDispatcher.Instance.AddListener(EventType.AugmentChargeEvent, param => Charge((float) param));
            EventDispatcher.Instance.AddListener(EventType.AugmentDrainEvent, param => Drain((float) param));
        }
        
        private void SetColor(AugmentUIData data, float duration) {
            augmentBackground.DOColor(data.primaryColor, duration);
            augmentSprite.DOColor(data.secondaryColor, duration);
        }
        
        private void Charge(float duration) {
            var anim = DOTween.To(() => slider.value, x => slider.value = x, 1f, duration).SetEase(Ease.Linear);
            anim.OnComplete(OnMaxCharge);
        }
        
        private void OnMaxCharge() {
            SetColor(_augmentData, 0.2f);
        }
        
        private void Drain(float duration) {
            SetColor(_noneData, 0.1f);
            var anim = DOTween.To(() => slider.value, x => slider.value = x, 0f, duration).SetEase(Ease.Linear);
        }

        
        private void Reset() {
            slider.value = 0f;
            SetColor(_noneData, 0.1f);
            foreach (var item in uiRenderers) {
                item.color = _noneData.primaryColor;
            }
        }
        
        private void OnElementChange(EmpowerType type) {
            _augmentData = uiData.GetByType(type);
            if (_augmentData == null) return;
            Reset();
            augmentSprite.sprite = _augmentData.icon;
            foreach (var item in uiRenderers) {
                item.color = _augmentData.primaryColor;
            }
        }
    }
}
