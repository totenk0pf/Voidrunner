using System;
using System.Collections.Generic;
using System.Globalization;
using Core.Events;
using TMPro;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace UI {
    public struct TextUIObj {
        public TextUI.TextType type;
        public float value;
    }
    
    public class TextUI : MonoBehaviour {
        [SerializeField] private List<TextItem> texts = new();

        public enum TextType {
            Experience
        }
        
        [Serializable]
        private struct TextItem {
            public TextMeshProUGUI text;
            public TextType type;
        }

        private void Start() {
            EventDispatcher.Instance
                .AddListener(EventType.UITextChangedEvent, msg => {
                    var cast = (TextUIObj)msg;
                    texts.Find(text => text.type == cast.type)
                        .text
                        .SetText(cast.value.ToString(CultureInfo.InvariantCulture));
                });
        }
    }
}
