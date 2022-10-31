using UnityEngine;

public class WorldItem : MonoBehaviour 
{
    private void OnPickup() {
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider other) {
        var x = other.transform.GetComponent<InventorySystem>();
        if (!x) return;
        OnPickup();
    }
}
