using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Object hold: every gun in game
 * Content: gun behaviour for raycast and projectile 
 */


public class GunCore : MonoBehaviour
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
        if (Input.GetMouseButtonDown(0) && gunAmmo >= 1 && clipAmount >= 1) 
        {
            gunAmmo--;
            GunOnShootEvent();
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            isReloading = true;
            StartCoroutine(Reload());
            GunOnReloadEvent();
        }

    }

    IEnumerator Reload() 
    {
        yield return new WaitForSeconds(this.gameObject.GetComponentInParent<Animator>().runtimeAnimatorController.animationClips[0].length);
        clipAmount--;
        gunAmmo = gunDefaultAmmo;
        isReloading = false;
    }

    public virtual void GunOnShootEvent() { }
    public virtual void GunOnReloadEvent() { }
}
