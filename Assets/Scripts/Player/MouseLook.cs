using Core.Events;
using UI;
using UnityEngine;
using EventType = Core.Events.EventType;

public class MouseLook : MonoBehaviour {
    
    public float lookSpeed;
    public bool isInverted;
    [SerializeField] private float camLimit;
    [SerializeField] private Transform gimbal;
    private Transform _transform;
    private Camera _camera;
    private Vector3 _lookVector;
    private bool _canLook;

    private void Awake() {
        _transform = transform;
        _camera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        this.AddListener(EventType.InventoryUIEvent, msg => OnInventoryUIEvent((InventoryUIMsg) msg));
    }

    private void OnInventoryUIEvent(InventoryUIMsg msg) {
        _canLook = msg.state;
    }

    private void LateUpdate() {
        // if (!_canLook) return;
        RotatePlayer();
        RotateCamera();
    }

    private void RotatePlayer() {
        float rot = lookSpeed * Time.deltaTime * Time.timeScale * Input.GetAxis("Mouse X");
        _transform.Rotate(_transform.up, rot);
    }

    private void RotateCamera() {
        float rot = lookSpeed * Time.deltaTime * Time.timeScale * (isInverted ? 1 : -1) * Input.GetAxis("Mouse Y");
        _lookVector.x += rot;
        _lookVector.x = Mathf.Clamp(_lookVector.x, -camLimit, camLimit);
        gimbal.rotation = Quaternion.Euler(_lookVector.x, gimbal.eulerAngles.y, 0f);
    }
}
