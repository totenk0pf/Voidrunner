using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Object hold: every gun in game
 * Content: gun behaviour for raycast and projectile 
 */


public class GunCore : MonoBehaviour,IGun
{
    [Header("Gun Info")]
    public float GunDamage;
    public int gunAmmo;
    public int clipAmount;
    public bool isReloading;
    //======== Gun Varibles Private ==========
    private int gunDefaultAmmo;
    // Start is called before the first frame update
    void Start()
    {
        gunDefaultAmmo = gunAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        //checking whether player have pressed right mouse button and whether gun switching animation playing
        if (Input.GetMouseButtonDown(0) && this.gameObject.GetComponentInParent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("GunSwitching")) 
        {
            //checking if there is any ammo left
            if(gunAmmo >= 1) 
            {
                //play gun switching animation
            }
            else 
            { 
                //play no ammo animation
            }
            
            
        }

        //if player input R key
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            //making sure if there is gun clip left
            if(clipAmount >= 1) 
            {
                //preventing from fire while reloading
                isReloading = true;
                //start reload couroutine
                StartCoroutine(Reload());
            }
            else 
            {
                //play no item animation
            }


        }

    }

    //funnction for animation event to shoot gun when player finish pulling gun out
    public void ShootGun() 
    {
        //decrease gun ammo
        gunAmmo--;
    }

    IEnumerator Reload() 
    {
        yield return new WaitForSeconds(this.gameObject.GetComponentInParent<Animator>().runtimeAnimatorController.animationClips[0].length);
        clipAmount--;
        gunAmmo = gunDefaultAmmo;
        isReloading = false;
    }

    void IGun.Fire()
    {
        
    }

    void IGun.Reload()
    {
        
    }
}
