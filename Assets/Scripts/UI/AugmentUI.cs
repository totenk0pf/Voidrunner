using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace UI {
    public enum AugmentType {
        None,
        Arc,
        Fire,
        Chem,
    }

    public class AugmentUI : MonoBehaviour {
        public AugmentType currentType;
        public float drainDuration;
        
        [TitleGroup("Data")]
        [SerializeField] private UIData uiData;
        [SerializeField] private Slider slider;
        [SerializeField] private float chargeDuration;
        private AugmentUIData _augmentData;
        private AugmentUIData _noneData;

        [TitleGroup("UI components")]
        [SerializeField] private Image augmentSprite;
        [SerializeField] private Image augmentBackground;
        [SerializeField] private List<Image> uiRenderers;

        [Button("Update augment")]
        private void UpdateAugment() {
            EventDispatcher.Instance.FireEvent(EventType.AugmentChangedEvent, currentType);
        }
        
        private void Awake() {
            _noneData = uiData.GetByType(AugmentType.None);
            _augmentData = uiData.GetByType(currentType);
            EventDispatcher.Instance.AddListener(EventType.AugmentChangedEvent, (e) => OnElementChange((AugmentType) e));
            EventDispatcher.Instance.AddListener(EventType.AugmentDrainEvent, (param) => Drain());
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.J)) {
                EventDispatcher.Instance.FireEvent(EventType.AugmentDrainEvent);
            }
        }

        private void SetColor(AugmentUIData data, float duration) {
            augmentBackground.DOColor(data.primaryColor, duration);
            augmentSprite.DOColor(data.secondaryColor, duration);
        }
        
        private void Charge() {
            var anim = DOTween.To(() => slider.value, x => slider.value = x, 1f, chargeDuration).SetEase(Ease.Linear);
            anim.OnComplete(OnMaxCharge);
        }
        
        private void OnMaxCharge() {
            SetColor(_augmentData, 0.2f);
        }
        
        private void Drain() {
            SetColor(_noneData, 0.1f);
            var anim = DOTween.To(() => slider.value, x => slider.value = x, 0f, drainDuration).SetEase(Ease.Linear);
            anim.OnComplete(Charge);
        }

        
        private void Reset() {
            slider.value = 0f;
            SetColor(_noneData, 0.1f);
            foreach (var item in uiRenderers) {
                item.color = _noneData.primaryColor;
            }
        }
        
        private void OnElementChange(AugmentType type) {
            _augmentData = uiData.GetByType(type);
            if (_augmentData == null) return;
            Reset();
            Charge();
            augmentSprite.sprite = _augmentData.icon;
            foreach (var item in uiRenderers) {
                item.color = _augmentData.primaryColor;
            }
        }
    }
}
