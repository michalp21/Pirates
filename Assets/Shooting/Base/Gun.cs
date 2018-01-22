using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Element
{
	NORMAL,
	FIRE,
	ICE,
	ACID,
	ELECTRIC,
	POISON
}

public enum ProjectileSurvive
{
	AIR,
	WATER,
	AIRWATER
}

public enum WeaponType
{
	CHILDING,
	NONCHILDING,
	MELEE
}

[RequireComponent (typeof (WeaponStatsBase))]
public class Gun : Weapon {
	public int currentLevel;
	public bool isSelf; //make into property when don't need inspector anymore? //as opposed to the opponent //isSelf will also be a variable in weaponstatsmanager
	public WeaponStatsBase weaponStats;
	public GameObject projectile = null;

	//Below variables should not be public
	protected float fireRate;               	// time between shots
	protected float nextFireTime = 0.0f;        // able to fire again on this frame
	protected Coroutine currentCoroutine;

    protected virtual void Start()
    {
		weaponStats = GetComponent<WeaponStatsBase>();
		fireRate = weaponStats.baseRate;
		currentCoroutine = null;
    }

	//Call initGun() after instantiating the GameObject with this Gun script attached
	protected void initGun(bool i)
	{
		isSelf = i;
	}

	public override void Attack() {
		Fire ();
	}

    // all guns handle firing a bit different so give it a blank function that each gun can override
    public virtual void Fire()
    {

    }

	// everything fires a single round the same
	protected virtual GameObject FireOneShot()
	{
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

			if (weaponStats.weaponType == WeaponType.CHILDING){
				newBullet.GetComponent<Transform> ().SetParent (weaponStats.muzzlePoint);
			}
			newBullet.GetComponent<Damager>().SetUp(weaponStats.usePooling); // send bullet info to spawned projectile
			return newBullet;
		}

		return null;
	}

	public void StartBoost ()
	{
		fireRate *= 1/3f;
		Health myHealth = gameObject.GetComponent<Health>();
		currentCoroutine = StartCoroutine (myHealth.Drain (Mathf.CeilToInt(myHealth.maxHealth / 20)));
	}
	
	public void StopBoost ()
	{
		fireRate = weaponStats.baseRate;
		if (currentCoroutine != null)
			StopCoroutine (currentCoroutine);
	}
}
