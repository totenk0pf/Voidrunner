using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(BoxCollider))]
public class ColliderToFit : MonoBehaviour {
    [MenuItem("Collab Toolkit/Collider/Fit to Children")]
    private static void FitToChildren() {
        foreach (GameObject rootGameObject in Selection.gameObjects) {
            if (rootGameObject.GetComponent<Collider>() == null) {
                rootGameObject.AddComponent<BoxCollider>();
            }
 
            var hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
              
            for (var i = 0; i < rootGameObject.transform.childCount; ++i) {
                Renderer childRenderer = rootGameObject.transform.GetChild(i).GetComponent<Renderer>();
                if (childRenderer == null) continue;
                if (hasBounds) {
                    bounds.Encapsulate(childRenderer.bounds);
                }
                else {
                    bounds    = childRenderer.bounds;
                    hasBounds = true;
                }
            }
              
            BoxCollider collider = (BoxCollider)rootGameObject.GetComponent<Collider>();
            collider.center = bounds.center - rootGameObject.transform.position;
            collider.size   = bounds.size;
        }
    }
}