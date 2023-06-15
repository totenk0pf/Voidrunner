using UnityEngine;

namespace Scenes.RoomVolume {
    public class TriggerKill : MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
                other.gameObject.GetComponent<Oxygen>().ReduceOxygen(99999999);
            }
        }
    }
}
