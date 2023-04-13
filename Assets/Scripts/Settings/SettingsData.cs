namespace Settings {
    public class SettingsData {
        public bool fullscreen;
        public bool particles;
        public bool postprocess;
        public float sensitivity;
        public int resolution;

        public float masterVolume;
        public float musicVolume;
        public float effectsVolume;
        public float voicesVolume;
        public bool captions;

        public SettingsData() {
            fullscreen    = true;
            particles     = true;
            postprocess   = true;
            sensitivity   = 0.5f;
            resolution    = 0;
            
            masterVolume  = 1.0f;
            musicVolume   = 1.0f;
            effectsVolume = 1.0f;
            voicesVolume  = 1.0f;
            captions      = true;
        }
    }
}