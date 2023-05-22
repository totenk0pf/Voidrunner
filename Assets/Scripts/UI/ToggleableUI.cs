using UnityEngine;

public class ToggleableUI : MonoBehaviour {
    public KeyCode key;
    [SerializeField] private GameObject rootToggleable;

    private void Update() {
        if (Input.GetKeyDown(key)) {
            rootToggleable.SetActive(!rootToggleable.activeSelf);
        }
    }
}
