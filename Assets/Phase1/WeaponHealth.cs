using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHealth : Health {

	public WeaponStatsBase weaponStats;
	public WeaponManager weaponManager;

	// Use this for initialization
	void Start () {
		weaponStats = GetComponentInChildren<WeaponStatsBase> (); //maybe change to 2nd parameter (See above)
		useObjectPooling = weaponStats.usePooling;
		myResistances = weaponStats.myResistances;
		maxHealth = weaponStats.health;

		weaponManager = GetComponentInParent<WeaponManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Hit (int damage, Element damageType)
	{
		base.Hit (damage, damageType);
		weaponManager.TakeDamage (damage);
	}

	protected override void onDrain ()
	{
		base.onDrain ();
		weaponManager.TakeDamage (DRAIN_PER_FRAME);
	}

	public override void Die ()
	{
		weaponManager.TakeDamage (health);
		health = 0;
		isDead = true;
		weaponManager.RemoveWeapon (gameObject.GetComponent<Gun> ());

		if (useObjectPooling)
		{
			ObjectPool.pool.PoolObject(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	protected override void onRegen ()
	{
		base.onRegen ();
		weaponManager.TakeDamage (-REGEN_PER_FRAME);
	}
}
