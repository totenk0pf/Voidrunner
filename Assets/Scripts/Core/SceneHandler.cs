using Core.Patterns;
using UnityEngine.SceneManagement;

namespace Core {
    public class SceneHandler : Singleton<SceneHandler> {
        public void StartGame() {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}