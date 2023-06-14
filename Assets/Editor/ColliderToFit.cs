using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(BoxCollider))]
public class ColliderToFit : MonoBehaviour {
    [MenuItem("Collab Toolkit/Collider/Fit to Children")]
    public static void FitToChildren() {
        foreach (GameObject root in Selection.gameObjects) {
            if (root.GetComponent<Collider>() == null) {
                root.AddComponent<BoxCollider>();
            }
 
            var hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
              
            for (var i = 0; i < root.transform.childCount; ++i) {
                Renderer childRenderer = root.transform.GetChild(i).GetComponent<Renderer>();
                if (childRenderer == null) continue;
                if (hasBounds) {
                    bounds.Encapsulate(childRenderer.bounds);
                }
                else {
                    bounds    = childRenderer.bounds;
                    hasBounds = true;
                }
            }
              
            BoxCollider collider = (BoxCollider)root.GetComponent<Collider>();
            collider.center = bounds.center - root.transform.position;
            collider.size   = bounds.size;
        }
    }
}