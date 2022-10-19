using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExecute : MonoBehaviour
{
    
    public Animator playerAnims;
    public ExecutionTrigger enemyToExecute;

    private void Start()
    {
        playerAnims = gameObject.GetComponentInParent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && enemyToExecute != null) 
        {
            //disable player controller
            playerAnims.SetTrigger(enemyToExecute.playerAnimationID);
            enemyToExecute.enemyAnims.SetTrigger("execution animation");
            this.enabled = false;
            
        }
    }

    public void OnExecuteEnd() 
    { 
        //enable player controller
    }
}
