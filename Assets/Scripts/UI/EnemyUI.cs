using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Slider healthBar;
    [SerializeField] private float animTime;
    
    [SerializeField] private string name;
    [SerializeField] private TextMeshProUGUI nameText;
    
    private Camera _cam; 

    private void Awake() {
        nameText.text = name;
        _cam          = Camera.main;
        healthBar.maxValue = 1;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private void Update()
    {
        var rotation = _cam.transform.rotation;
        var worldPos = transform.position + rotation * Vector3.forward;
        var worldUp = rotation * Vector3.up;
        transform.LookAt(worldPos, worldUp);
    }

    public void UpdateBar(float health) {
        DOTween.To(() => healthBar.value, x => healthBar.value = x, health, animTime);
    }
}
