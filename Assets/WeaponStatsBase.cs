using UnityEngine;
using System.Collections;

public abstract class WeaponStatsBase
{
	public int currentLevel;
	public string weaponName;                         // Name of this weapon
	public int rarity;
	public Transform muzzlePoint;        // the muzzle point for this gun, where you want bullets to be spawned
	public WeaponType typeOfWeapon;             // type of weapon, used to determine how the trigger acts
	public bool usePooling;             // do we want to use object pooling or instantiation
	public bool infiniteAmmo;		//whether there is infinite ammo
	public int maxLevel;	
	public ProjectileSurvive psurvive;

	public Damage damage = new Damage();        // the damage and type of damage this gun does
	public Resistance myResistances = new Resistance();
	public int maxPenetration;              // maximum amount of hits detected before the bullet is destroyed
	public float targetRange;
	public int disappearRange;
	public float spread;                 // current spread of the gun
	public float projectileSpeed;      // speed that projectile flies at
	public float projectileLifeTime;     // how long before the projectile is considered gone and recycleable
	public float baseFireRate;
}
