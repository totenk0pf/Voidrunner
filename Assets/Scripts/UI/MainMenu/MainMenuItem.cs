using System;
using Core.Events;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using EventType = Core.Events.EventType;

namespace UI.MainMenu {
    [RequireComponent(typeof(BoxCollider))]
    public class MainMenuItem : SerializedMonoBehaviour {
        public string title;
        public string desc;
        public UnityAction action;
    }
}