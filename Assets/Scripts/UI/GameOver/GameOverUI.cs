using System;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using EventType = Core.Events.EventType;

namespace UI.GameOver {
    public class GameOverUI : MonoBehaviour {
        private void Awake() {
            this.AddListener(EventType.ToggleDeathUI, _ => {
                gameObject.SetActive(true);
            });
            
            gameObject.SetActive(false);
        }

        public void Retry() {
            EventDispatcher.Instance.FireEvent(EventType.OnPlayerRespawn);
            gameObject.SetActive(false);
        }

        public void ToMenu() {
            SceneManager.LoadScene(0);
        }
    }
}
