using Core.Patterns;

namespace Settings {
    public class SettingsManager : Singleton<SettingsManager> {
        private SettingsData _currentSettings;

        private void Awake() {
            _currentSettings = FileHandler.LoadFromFile();
        }

        public void SaveSettings() {
            FileHandler.WriteSettings(_currentSettings);
        }

        public void ResetSettings() {
            FileHandler.WriteSettings(new SettingsData());
        }
    }
}