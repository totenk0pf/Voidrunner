using UnityEngine;

public class EntityBase : MonoBehaviour, IEntity
{
    protected string entityName;
    protected float entityHP;

    void IEntity.Die() {
        throw new System.NotImplementedException();
    }

    void IEntity.TakeDamage() {
        throw new System.NotImplementedException();
    }
}
