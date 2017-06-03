using UnityEngine;
using System.Collections;

//Guns that shoot things that are not childed to the guns
public class Gun_Nonchilding : Gun {
    public GameObject projectile = null;        // projectile prefab... whatever this gun shoots
    
    protected override void Start()
    {
        base.Start();
        //SetupBulletInfo(); // set a majority of the projectile info
    }

	protected override GameObject FireOneShot () {
		if (GetComponentInChildren<Target> ().canFire) {
			Vector3 pos = weaponStats.muzzlePoint.position; // position to spawn bullet is at the muzzle point of the gun       
			Quaternion rot = weaponStats.muzzlePoint.rotation; // spawn bullet with the muzzle's rotation

			//bulletInfo.spread = weaponStats.spread; // set this bullet's info to the gun's current spread
			GameObject newBullet;

			if (weaponStats.usePooling)
			{
				newBullet = ObjectPool.pool.GetObjectForType(projectile.name, false);
				newBullet.transform.position = pos;
				newBullet.transform.rotation = rot;
				if (isSelf == true)
					newBullet.layer = 9;
				else
					newBullet.layer = 10;
			}
			else
			{
				newBullet = Instantiate(projectile, pos, rot) as GameObject; // create a bullet
				newBullet.name = projectile.name;
				if (isSelf == true)
					newBullet.layer = 9;
				else
					newBullet.layer = 10;
			}

			newBullet.GetComponent<Transform> ().SetParent (weaponStats.muzzlePoint);
			newBullet.GetComponent<Damager>().SetUp(weaponStats.usePooling); // send bullet info to spawned projectile
			//Debug.Log ("returned bullet");
			return newBullet;
		}

		//Debug.Log ("returned null");
		return null;
	}
}