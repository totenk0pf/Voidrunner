using UnityEditor;
using UnityEngine;

public class SceneCleanup : MonoBehaviour {
    [MenuItem("Collab Toolkit/Collider/Clean selection up")]
    private static void CleanSelection() {
        foreach (GameObject root in Selection.gameObjects) {
            var total = root.transform.GetComponentsInChildren<Collider>(true);
            if (!EditorUtility.DisplayDialog("Clean selection up",
                                             "Are you sure you want to delete all colliders from selected objects?",
                                             "Delete",
                                             "Cancel")) continue;
            Undo.RecordObjects(total, "Collider cleanup");
            for (var i = 0; i < total.Length; i++) {
                PrefabUtility.RecordPrefabInstancePropertyModifications(total[i]);
                DestroyImmediate(total[i]);
            }
        }
    }
}