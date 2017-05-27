using UnityEngine;
using System.Collections;

public class SimpleGunProjectile : Projectile
{
	protected override void OnTriggerEnter(Collider other)
	{
		// if we hit water... kill the bullet IF it won't survive underwater (not fully implemented yet)
		/*if (other.gameObject.CompareTag("Water") &&
		   (myInfo.psurvive == ProjectileSurvive.AIR))
		{
			CancelInvoke("Recycle");
			Recycle();
		}*/

		//do one for air too

		if (other.gameObject.tag.Contains("Weapon"))
		{
			//coll.GetComponent<healthScript>().health -= 1;

			Health hitObject = other.gameObject.GetComponent<Health>();
			hitObject.Hit(damage, damageType); // send bullet info to hit object's health component
			
			hitCount++; // add a hit
			
			if (hitCount >= maxPenetration)
			{
				CancelInvoke("Recycle");
				Recycle(); // if hit count exceeds max hits.... kill the bullet
			}
		}
	}
}