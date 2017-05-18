using UnityEngine;
using System.Collections;

/// <summary>
/// component 'Shotgun'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/Shotgun")]
public class Gun_Shotgun : Gun_Physical
{
    public int pelletCount = 8;

   
    public override void Fire()
    {
        if (nextFireTime < Time.time)
        {

            for (int i = 0; i <= pelletCount; i++)
            {
                FireOneShot();
            }

            nextFireTime = Time.time + fireRate;
        }
    }


}
