using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ReplaceMaterialWizard : ScriptableWizard {
    public Material originalMaterial;
    public Material newMaterial;
    
    private Renderer[] _renderers = {};
    private Renderer[] _filtered = {};

    [MenuItem("Collab Toolkit/Replace materials")]
    private static void CreateWizard() {
        DisplayWizard<ReplaceMaterialWizard>("Replace materials", "Replace");
    }

    private void Awake() {
        _renderers = FindObjectsOfType<Renderer>();
    }

    private void OnWizardCreate() {
        if (originalMaterial == null) {
            errorString = "Please select a material to replace.";
        }
        if (newMaterial == null) {
            errorString = "Please assign the new material.";
        }
        if (!originalMaterial || !newMaterial) return;
        Undo.RecordObjects(_filtered, "Replace materials");
        for (var i = 0; i < _filtered.Length; i++) {
            _filtered[i].sharedMaterial = newMaterial;
        }
    }

    private void OnWizardUpdate() {
        _filtered  = _renderers.Where(x => x.sharedMaterial == originalMaterial).ToArray();
        if (_filtered != null && originalMaterial != null)
        helpString = $"Found {_filtered.Length} objects with {originalMaterial.name} material in scene.";
    }
}