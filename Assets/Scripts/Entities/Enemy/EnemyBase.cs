using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : EntityBase
{
    public float enemyDamage; 
    public float enemySpeed;
    public float enemyHP;

    [Header("Enemy UI")] 
    [SerializeField] private EnemyUI _ui;

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
        
        if (!_ui) Debug.LogWarning("Missing EnemyUI ref in " + this);
        _ui.healthBar.maxValue = enemyHP;
        _ui.healthBar.value = enemyHP;
    }

    private void Update() {
        if (_currentHP <= 0) {
            Die();
        }
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
        _ui.healthBar.value = _currentHP;
    }
}
