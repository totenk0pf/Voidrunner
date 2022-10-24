using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    [Serializable]
    public class AugmentUIData {
        public AugmentType type;
        public Sprite icon;
        public Color primaryColor;
        public Color secondaryColor;
    }

    [CreateAssetMenu(fileName = "UIData", menuName = "ScriptableObjects/UIData", order = 1)]
    public class UIData : ScriptableObject {
        public List<AugmentUIData> augmentData = new();
        public Color defaultColor;
        public Color disabledColor;

        public AugmentUIData GetByType(AugmentType type) {
            return augmentData.Find(x => x.type == type);
        }
    }
}