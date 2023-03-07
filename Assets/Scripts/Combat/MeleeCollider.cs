using System;
using System.Collections;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class MeleeCollider : MonoBehaviour
{
    public List<EnemyBase> Enemies { get; } = new ();
    [SerializeField] MeleeOrder order;

    public MeleeOrder Order => order;

    public void ResetEnemiesList() {
        Enemies.Clear();
    }

    private void OnTriggerEnter(Collider col) {
        var enemy = GetEnemy(col);
        if (enemy) Enemies.Add(enemy);
    }

    private void OnTriggerExit(Collider col) {
        var enemy = GetEnemy(col);
        if (enemy) Enemies.Remove(enemy);
    }

    private EnemyBase GetEnemy(Collider col) {
        var enemy = col.GetComponent<EnemyBase>();
        return !enemy || Enemies.Contains(enemy) ? null : enemy;
    }
}
