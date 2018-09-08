using System;
using System.IO;
using UnityEngine;


public class cool : MonoBehaviour
{
    config cof = JsonUtility.FromJson<config>(File.ReadAllText(Environment.CurrentDirectory + "/QMods/techpistol/config.json"));
    void OnParticleCollision(GameObject taget)
    {
        try
        {
            int sphere = UWE.Utils.OverlapSphereIntoSharedBuffer(taget.transform.position, cof.CannonExplosionDamageRange,- 1, QueryTriggerInteraction.UseGlobal);
            for (int i = 0; i < sphere; i++)
            {
                ;
                GameObject Root = UWE.Utils.GetEntityRoot(UWE.Utils.sharedColliderBuffer[i].gameObject);
                if (Root != null && Root.GetComponent<LiveMixin>() != null)
                {
                    Root.GetComponent<LiveMixin>().TakeDamage(cof.CannonDamage, Root.transform.position, DamageType.Explosive, null);
                }
            }
        }
        catch
        { 
        }
    }
}
