using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Logging;
using Extensions;
using Sirenix.OdinInspector;
using StaticClass;
using UnityEngine;
using EventType = Core.Events.EventType;

public class MeleeCollision : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [ReadOnly] [SerializeField] private List<Collision> enemyList;

    private void OnCollisionEnter(Collision collision)
    {
        // NCLogger.Log($"name: {collision.collider.name}");
        if (!CheckLayerMask.IsInLayerMask(collision.collider.gameObject, enemyLayer)) return;
        enemyList.Add(collision);
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if(enemyList.Contains(collision)) 
            enemyList.Remove(collision);
    }

    public void SpawnBlood(int particlePerEnemy)
    {
        NCLogger.Log($"enemy to spawn blood on {enemyList.Count}");
        if (enemyList.Count == 0) return;
        foreach (var col in enemyList) {
            for (var i = 0; i < particlePerEnemy; i++) {
                // if(col.contacts.Length != 0) 
                    // this.FireEvent(EventType.SpawnParticleREDEvent, new ParticleCallbackData(col.contacts[0].normal, col.contacts[0].point));
            }
        }
    }
}