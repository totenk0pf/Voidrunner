using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Combat.Melee
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]

    public class MeleeBase : MonoBehaviour,IMeleeWeapon
    {
        protected float damage;

        private void Start()
        {
            //disable rigibody
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        private void OnTriggerEnter(Collider objCollided)
        {
            //checking whether object has enemy tag
            if (objCollided.CompareTag("Enemy")) 
            { 
                //damage enemy and play effect
            }
        }

        private void OnTriggerExit(Collider objCollided)
        {
            
        }

        void IMeleeWeapon.AltFire()
        {
            
        }

        void IMeleeWeapon.Fire()
        {

        }
    }
}
