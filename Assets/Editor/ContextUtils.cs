using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ContextUtils {
    [MenuItem("GameObject/Collab Toolkit/Parent and Fit", false, 0)]
    private static void ParentAndFit(MenuCommand cmd) {
        if (cmd.context) {
            EditorApplication.update -= CallOnce;
            EditorApplication.update += CallOnce;
            return;
        }

        var selection = Selection.transforms;
        if (selection.Length == 0) return;

        var renderers = new List<Renderer>();
        Bounds bounds = new();
        var boundsInitialized = false;
        for (var i = 0; i < selection.Length; i++) {
            if (AssetDatabase.Contains(selection[i].gameObject)) continue;

            renderers.Clear();
            selection[i].GetComponentsInChildren(renderers);

            for (var j = renderers.Count - 1; j >= 0; j--) {
                if (boundsInitialized) bounds.Encapsulate(renderers[j].bounds);
                else {
                    bounds            = renderers[j].bounds;
                    boundsInitialized = true;
                }
            }
        }


        GameObject parentObj = new("Group") {
            transform = {
                parent = selection[0].parent
            }
        };
        Transform newParent = parentObj.transform;
        newParent.position = bounds.center;
        //newParent.position -= new Vector3( 0f, bounds.extents.y, 0f ); // Move pivot to the bottom

        Undo.RegisterCreatedObjectUndo(parentObj, "Parent Selected");
        for (var i = 0; i < selection.Length; i++) {
            if (AssetDatabase.Contains(selection[i].gameObject)) continue;
            Undo.SetTransformParent(selection[i], newParent, "Parent Selected");
        }

        Selection.activeTransform = newParent;
        ColliderToFit.FitToChildren();
    }

    private static void CallOnce() {
        EditorApplication.update -= CallOnce;
        ParentAndFit(new MenuCommand(null));
    }

    [MenuItem("GameObject/Collab Toolkit/Parent To", false, 0)]
    private static void ParentToObject() {
        
    }

    private static List<GameObject> GetAllObjectsInScene() {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
            if (go.hideFlags != HideFlags.None) continue;

            if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab ||
                PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
                continue;

            objectsInScene.Add(go);
        }

        return objectsInScene;
    }
}