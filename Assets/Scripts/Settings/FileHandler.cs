using System.IO;
using Core.Logging;
using UnityEngine;
using Newtonsoft.Json;

namespace Settings {
    public static class FileHandler {
        private static string settingsPath = Application.persistentDataPath;
        private const string fileName = "settings.json";
        private static string fullPath = Path.Combine(Path.GetFullPath(settingsPath), fileName);
        
        public static SettingsData LoadFromFile() {
            var test = Path.Combine(Path.GetFullPath(settingsPath), fileName);
            NCLogger.Log($"Reading data from: {fullPath}");
            SettingsData data = new();
            if (!File.Exists(fullPath)) {
                NCLogger.Log($"Settings file not found, creating a new one.", LogLevel.WARNING);
                WriteSettings(data);
            }
            var fileData = File.ReadAllText(fullPath);
            SettingsData parsedJson = JsonConvert.DeserializeObject<SettingsData>(fileData);
            return parsedJson;
        }

        public static void WriteSettings(SettingsData data) {
            var export = JsonConvert.SerializeObject(data);
            File.WriteAllText(fullPath, export);
        }
    }
}