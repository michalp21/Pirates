using UnityEngine;
using System.Collections;

public class Gun_SingleShot : Gun_Physical
{
    public override void Fire()
    {
		if (nextFireTime < Time.time) {
			FireOneShot ();
			nextFireTime = Time.time + fireRate;
		}
    }   
}
