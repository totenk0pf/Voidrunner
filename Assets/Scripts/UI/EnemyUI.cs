using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour {
    [SerializeField] private string name;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI nameText;

    private void Awake() {
        nameText.text = name;
        healthBar.value = 1f;
    }
}
