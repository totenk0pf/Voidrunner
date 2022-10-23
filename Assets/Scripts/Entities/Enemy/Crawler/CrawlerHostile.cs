using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class CrawlerHostile : EnemyState
{
    public float minDelayBeforeAttack;
    public float maxDelayBeforeAttack;
    [SerializeField] private EnemyState _nextState; 

    private Vector3 target;
    private Strafe currentStrafe = Strafe.Left;

    [HideInInspector] public bool _canAttack = false;

    private enum Strafe {
        Left,
        Right
    }

    public override EnemyState RunCurrentState() {
        if (!_canAttack) {
            Agent.SetDestination(RotateAround(currentStrafe));
            StartCoroutine(StartAttack());

            NavMeshHit hit;
            Agent.FindClosestEdge(out hit);

            //magic number
            if (Vector3.Distance(transform.position, hit.position) < 1.4f) {

                switch (currentStrafe) {
                    case Strafe.Left:
                        currentStrafe = Strafe.Right;
                        break;

                    case Strafe.Right:
                        currentStrafe = Strafe.Left;
                        break;
                }
            }
        }

        else {
            _canAttack = false;
            return _nextState;
        }

        return this;
    }

    private Vector3 RotateAround(Strafe strafeDir) {
        var offset = Vector3.zero;

        if (strafeDir == Strafe.Left) {
            offset = transform.position - target;
        }

        else {
            offset = target - transform.position;
        }

        return transform.position + Vector3.Cross(offset, Vector3.up);
    }

    IEnumerator StartAttack() {
        yield return new WaitForSeconds(Random.Range(minDelayBeforeAttack, maxDelayBeforeAttack));
        _canAttack = true;
    }

    public override void OnTriggerEnter(Collider other) {
        
    }

    public override void OnTriggerStay(Collider other) {
        if (other.tag == "Player") target = other.transform.position; 
    }
}
