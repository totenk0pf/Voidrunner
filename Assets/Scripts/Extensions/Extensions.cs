using System;
using System.Collections.Generic;
using Core.Logging;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class DictionaryExtension
    {
        public static T Previous<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) - 1;
            return (0==j) ? Arr[j] : Arr[^1];            
        }
    
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length==j) ? Arr[0] : Arr[j];            
        }
    
        public static T Last<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            return Arr[^1];            
        }
    }

    public static class ConeCastExtension
    {
        public static RaycastHit[] ConeCastAll(this Physics physics, Vector3 origin, float maxRadius, Vector3 direction, float maxDistance, float coneAngle)
        {
            RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin - new Vector3(0,0,maxRadius), maxRadius, direction, maxDistance);
            List<RaycastHit> coneCastHitList = new List<RaycastHit>();
        
            if (sphereCastHits.Length > 0)
            {
                for (int i = 0; i < sphereCastHits.Length; i++)
                {
                    Vector3 hitPoint = sphereCastHits[i].point;
                    Vector3 directionToHit = hitPoint - origin;
                    float angleToHit = Vector3.Angle(direction, directionToHit);

                    if (angleToHit < coneAngle)
                    {
                        coneCastHitList.Add(sphereCastHits[i]);
                    }
                }
            }

            RaycastHit[] coneCastHits = new RaycastHit[coneCastHitList.Count];
            coneCastHits = coneCastHitList.ToArray();

            return coneCastHits;
        }
    
        public static void ConeCastEnemy(this Physics physics, ref List<EnemyBase> enemies, int loopCount, Vector3 origin, Vector3 direction, float maxDistance, float coneAngle, LayerMask layerMask)
        {
            for (int i = 0; i < loopCount; i++)
            {
                //Randomize shot in 3D from the wanted direction
                var randDir = Quaternion.AngleAxis(-Random.Range(-coneAngle, coneAngle), Vector3.forward) * direction;
                randDir = Quaternion.AngleAxis(-Random.Range(-coneAngle, coneAngle), Vector3.up) * randDir;
                randDir = Quaternion.AngleAxis(-Random.Range(-coneAngle, coneAngle), Vector3.right) * randDir;

                var isHit = Physics.Raycast(origin, randDir, out var hit,  maxDistance, layerMask);
                Debug.DrawLine(origin, origin + randDir * 10, Color.yellow, 5f);

                if(isHit) enemies.Add(hit.collider.GetComponent<EnemyBase>());
            }
        }
    }
}