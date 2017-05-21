using UnityEngine;
using System.Collections;

public class Gun_Physical : Gun {
    public GameObject projectile = null;        // projectile prefab... whatever this gun shoots
    
    protected override void Start()
    {
        base.Start();
        SetupBulletInfo(); // set a majority of the projectile info
    }
	
	protected override void FireOneShot () {
		Vector3 pos = weaponStats.muzzlePoint.position; // position to spawn bullet is at the muzzle point of the gun       
		Quaternion rot = weaponStats.muzzlePoint.rotation; // spawn bullet with the muzzle's rotation

		bulletInfo.spread = weaponStats.spread; // set this bullet's info to the gun's current spread
        GameObject newBullet;

        if (weaponStats.usePooling)
        {
            newBullet = ObjectPool.pool.GetObjectForType(projectile.name, false);
            newBullet.transform.position = pos;
            newBullet.transform.rotation = rot;
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

        newBullet.GetComponent<Projectile>().SetUp(bulletInfo); // send bullet info to spawned projectile
    }

	public override void Select () {
		isSelected = true;
	}

	public override void DeSelect () {
		isSelected = false;
	}

	public override void StartBoost () {
		fireRate *= .5f;
	}
	
	public override void StopBoost () {
		fireRate = weaponStats.baseFireRate;
	}
}