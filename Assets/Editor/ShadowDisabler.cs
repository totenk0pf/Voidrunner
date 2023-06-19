using UnityEngine;
using UnityEditor;

public class ShadowDisabler : MonoBehaviour {
    [MenuItem("Collab Toolkit/Collider/Disable light shadows")]
    public static void DisableLightShadows() {
        var lights = FindObjectsOfType<Light>();
        if (!EditorUtility.DisplayDialog("Hold the fuck up!",
                                         "Doing this will turn off ALL shadow settings for every light in the scene.\n" +
                                         "Are you sure you want to continue?", "Floor it", "Cancel")) return;
        Undo.RecordObjects(lights, "Remove shadow from lights");
        foreach (Light light in lights) {
            light.shadows = LightShadows.None;
        }
    }    
}