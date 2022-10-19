using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * attach into execute trigger (child of enemy)
 * enable script when enemy at low health
 */

public class ExecutionTrigger : MonoBehaviour
{
    public AnimationClip playerExecuteAnimation;
    public Animator enemyAnims;
    public string playerAnimationID;
    private PlayerExecute _playerEx;

    private void Start()
    {
        enemyAnims = gameObject.GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player")) 
        {
            if(_playerEx == null) 
            {
                _playerEx = player.GetComponent<PlayerExecute>();
            }

            _playerEx.enabled = true;
            _playerEx.enemyToExecute = this;
            //use vector 3 to adjust player position to suitable with the enviroment
            player.transform.position = new Vector3();
        }
    }

    private void OnTriggerExit(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            _playerEx.enabled = false;
            _playerEx.enemyToExecute = null;
        }
    }

}
