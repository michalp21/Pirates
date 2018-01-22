using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// component 'Gun_BurstFire'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/Gun_BurstFire")]
public class Gun_BurstFire : Gun
{
    public int burstCount = 3;
    public float burstLag = 0.1f;


    public override void Fire()
    {
        if (nextFireTime < Time.time)
        {
            StartCoroutine(BurstFire());
            
            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator BurstFire()
    {
        for (int i = 1; i <= burstCount; i++)
        {
            FireOneShot();

            yield return new WaitForSeconds(burstLag);
        }

    }


}
