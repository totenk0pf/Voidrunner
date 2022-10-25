using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Slider healthBar;
    
    [SerializeField] private string name;
    [SerializeField] private TextMeshProUGUI nameText;
    
    private Camera _cam; 

    private void Awake() {
        nameText.text = name;

        _cam = Camera.main;
        if (_cam == null) Debug.LogError("Missing Camera Ref in " + this);
    }

    private void Update()
    {
        //Update UI Pos
        var rotation = _cam.transform.rotation;
        var worldPos = transform.position + rotation * Vector3.forward;
        var worldUp = rotation * Vector3.up;
        transform.LookAt(worldPos, worldUp);
    }
}
