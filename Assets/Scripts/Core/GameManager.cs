using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using Core.Patterns;
using DG.Tweening;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace Core {
    public class GameManager : Singleton<GameManager> {
        [SerializeField] private GameManagerData data;
        [ShowInInspector] [ReadOnly] private float _currentScale;
        private bool _gameLoaded;

        private void Awake() {
            this.AddListener(EventType.InventoryToggleEvent, msg => {
                var x = (InventoryToggleMsg) msg;
                ModifyTimescale(x.state ? TimescaleType.Slow : TimescaleType.Default);
            });
        }

        public void ModifyTimescale(TimescaleType type) {
            if (!data) return;
            var targetScale = data.scaleDict.GetValueOrDefault(type, 1f);
            var x = DOTween.To(
                () => Time.timeScale,
                x => {
                    Time.timeScale      = x;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                },
                targetScale,
                data.scaleModTime).SetUpdate(true).SetEase(Ease.OutExpo);
            _currentScale = targetScale;
        }

        public void Quit() {
            Application.Quit();
        }

        public void StartGame() {
            StartCoroutine(LoadAsyncScene());
        }

        private IEnumerator LoadAsyncScene() {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = false;
            while (!asyncLoad.isDone) {
                NCLogger.Log(asyncLoad.progress);
                if (asyncLoad.progress >= 0.9f) {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
            _gameLoaded = true;
        }
    }
}