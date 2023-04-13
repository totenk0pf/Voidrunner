using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UI.MainMenu {
    [RequireComponent(typeof(BoxCollider))]
    public class MainMenuItem : SerializedMonoBehaviour {
        public string title;
        public string desc;
        public bool disableOnClick;
        public List<UnityAction> action;
    }
}