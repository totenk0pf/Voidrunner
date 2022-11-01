using System;
using System.Collections.Generic;
using System.Linq;
using Core.Logging;
using Core.Patterns;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Items {
    public class ItemManager : Singleton<ItemManager> {
        [TitleGroup("Items")]
        [ShowInInspector] public List<ItemData> itemList;

        [TitleGroup("Items settings")]
        [FolderPath(ParentFolder = "Assets/Resources")] [SerializeField]
        private string itemDataPath;

        [HorizontalGroup("Buttons")]
        [Button("Load items")]
        private void LoadItems() {
            try {
                var x = Resources.LoadAll<ItemData>(itemDataPath);
                itemList = x.ToList();
                NCLogger.Log($"Loaded {x.Length} items.");
            } catch (Exception e) {
                NCLogger.Log(e.Message, LogLevel.ERROR);
            }
        }
        
        [HorizontalGroup("Buttons")]
        [Button("Validate items")]
        private void ValidateItems() {
            try {
                var count = itemList.RemoveAll(x => x.behaviour == null);
                NCLogger.Log($"Removed {count} items with null behaviour.");
            } catch (Exception e) {
                NCLogger.Log(e.Message, LogLevel.ERROR);
            }
        }

        private void OnValidate() {
            LoadItems();
            ValidateItems();
        }
    }
}