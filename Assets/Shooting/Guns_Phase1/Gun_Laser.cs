using UnityEngine;
using System.Collections;

//Guns that shoot things that are childed to the guns
public class Gun_Laser : Gun_Childing
{
	private GameObject myLaser = null;

    public override void Fire()
    {
		if (nextFireTime < Time.time) {
			if (myLaser == null) {
				myLaser = FireOneShot ();
			}
			nextFireTime = Time.time + fireRate;
		}
    }   
}
