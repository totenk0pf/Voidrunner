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
        if (Input.GetMouseButtonDown(0)) 
        {
            if(gunAmmo >= 1) 
            {
                //play gun switching animation
            }
            else 
            { 
                //play no ammo animation
            }
            
            
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            if(clipAmount >= 1) 
            {
                isReloading = true;
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
        throw new System.NotImplementedException();
    }

    void IGun.Reload()
    {
        throw new System.NotImplementedException();
    }
}
