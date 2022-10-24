using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : EntityBase
{
    public float enemyDamage; 
    public float enemySpeed;
    public float enemyHP;
    public string enemyName;

    [Header("Enemy UI")]
    public GameObject UI;
    public Slider healthBar;
    public TextMeshProUGUI enemyNameText;

    private float _currentHP;

    //Modify NavMeshAgent Values in Inspector
    protected NavMeshAgent _navAgent;

    private void Start() {
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.speed = enemySpeed;

        if (!_navAgent.isOnNavMesh) {
            Debug.LogWarning("No NavMesh is bound to Enemy");
        }

        _currentHP = enemyHP;
        healthBar.maxValue = enemyHP;
        enemyNameText.text = enemyName;
    }

    private void Update() {
        if (_currentHP <= 0) {
            Die();
        }

        //Make UI lookat
        UI.transform.LookAt(UI.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        //Update healthbar
        healthBar.value = enemyHP;
    }

    public virtual void Attack() {
        throw new System.NotImplementedException();
    }

    //Moving that takes Transform as arguement 
    public virtual void Move(Transform destination) {
        _navAgent.SetDestination(destination.position);
    }

    //Moving that takes Vector3 as arguement 
    public virtual void Move(Vector3 destination) {
        _navAgent.SetDestination(destination);
    }

    public virtual void Die() {
        Destroy(this);
    }

    public virtual void TakeDamage(float ammount) {
        _currentHP -= ammount;
    }
}
