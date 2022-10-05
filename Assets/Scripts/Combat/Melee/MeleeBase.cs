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
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        void IMeleeWeapon.AltFire()
        {
            
        }

        void IMeleeWeapon.Fire()
        {

        }
    }
}
