using System;
using TMPro;
using UnityEngine;

namespace UI.MainMenu {
    public class MainMenuTooltip : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private GameObject tooltip;
        [SerializeField] private Vector2 offset;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private Canvas canvas;
        private const string titleFormat = "$ {0}";
        private const string descFormat = ">> {0}";
        private MainMenuItem _currentItem;
        
        private void Awake() {
            tooltip.SetActive(false);
        }

        private void UpdateTooltip(bool state, string title = "", string desc = "") {
            tooltip.SetActive(state);
            titleText.text = String.Format(titleFormat, title);
            descText.text  = String.Format(descFormat, desc);
        }

        private void Update() {
            var pos = (Vector2) Input.mousePosition + offset;
            transform.position = uiCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, canvas.planeDistance));
            var ray = uiCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit)) {
                _currentItem = hit.collider.GetComponent<MainMenuItem>();
                if (!_currentItem) return;
                UpdateTooltip(true, _currentItem.title, _currentItem.desc);
            } else {
                UpdateTooltip(false);
            }
            if (Input.GetMouseButtonDown(0) && _currentItem) {
                _currentItem.action.Invoke();
            }
        }
    }
}