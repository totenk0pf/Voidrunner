using System;
using System.Collections;
using Core.Logging;
using UnityEngine;


public class PooledObjectCallbackData
{
    public Vector3 position;

    public PooledObjectCallbackData(Vector3 position) {
        this.position = position;
    }

    protected PooledObjectCallbackData()
    {
        throw new NotImplementedException();
    }
}


public class PooledObjectBase : MonoBehaviour, IPooledCore<PooledObjectBase>
{
    protected bool canRelease;
    protected Action<PooledObjectBase> KillAction;

    protected virtual void Start()
    {
        canRelease = false;
        // transform.position = new Vector3(999, 999, 999);
    }

    public virtual void Init(PooledObjectCallbackData data, Action<PooledObjectBase> killAction)
    {
        throw new NotImplementedException();
    }

    public virtual IEnumerator RunRoutine()
    {
        while (!canRelease) {
            yield return null;
        }
        NCLogger.Log($"kill {gameObject.name}");
        KillAction(this);
        yield return null;
    }
}
