using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : EntityBase
{
    public float enemyDamage; 
    public float enemySpeed;
    public float enemyHP;
    protected float currentHp;

    [SerializeField] private EnemyUI ui;
    protected NavMeshAgent navAgent;

    private void Start() {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = enemySpeed;

        if (!navAgent.isOnNavMesh) {
            Debug.LogWarning("No NavMesh is bound to Enemy");
        }

        currentHp = enemyHP;
        
        ui.healthBar.maxValue = enemyHP;
        ui.healthBar.value = enemyHP;
    }

    private void Update() {
        if (currentHp <= 0) {
            Die();
        }
    }   

    public virtual void Attack() {
        throw new System.NotImplementedException();
    }

    public virtual void Move(Transform destination) {
        navAgent.SetDestination(destination.position);
    }
 
    public virtual void Move(Vector3 destination) {
        navAgent.SetDestination(destination);
    }

    public virtual void Die() {
        Destroy(gameObject);
    }

    public virtual void TakeDamage(float amount) {
        currentHp -= amount;
        ui.UpdateBar(amount);
    }
}
