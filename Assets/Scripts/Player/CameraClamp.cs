using UnityEngine;

namespace Player {
    public class CameraClamp : MonoBehaviour {
        [SerializeField] private float offset;
        [SerializeField] private float shiftSpeed;
        [SerializeField] private Transform gimbal;
        [SerializeField] private LayerMask whatIsLevel;
        private Camera _cam;
        private Vector3 _dist;

        private void Awake() {
            _cam  = Camera.main;
            _dist = _cam.transform.localPosition;
        }

        private void Update() {
            Vector3 dir = (_cam.transform.position - gimbal.position).normalized;
            if (Physics.Raycast(gimbal.position, 
                                dir, 
                                out RaycastHit hit,
                                _dist.magnitude,
                                whatIsLevel, 
                                QueryTriggerInteraction.Ignore)) {
                _cam.transform.position = Vector3.Lerp(_cam.transform.position, 
                                                       hit.point + dir * offset, 
                                                       shiftSpeed * Time.deltaTime);
            } else {
                _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition,
                                                       _dist,
                                                       shiftSpeed * Time.deltaTime
                );
            }
        }
    }
}