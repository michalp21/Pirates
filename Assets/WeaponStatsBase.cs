using UnityEngine;
using System.Collections;

//Separate weapon stuff from projectile stuff into respective classes. Get rid of the current bestowing/initializing.
public abstract class WeaponStatsBase : MonoBehaviour
{
	public int currentLevel;
	public string weaponName;                         // Name of this weapon
	public int rarity;
	public Transform muzzlePoint;        // the muzzle point for this gun, where you want bullets to be spawned
	public WeaponType typeOfWeapon;             // type of weapon, used to determine how the trigger acts
	public bool usePooling;             // do we want to use object pooling or instantiation
	public bool infiniteAmmo;		//whether there is infinite ammo
	public int maxLevel;
	public int health;
	public Resistance myResistances = new Resistance();
	public float targetRange;
	public int disappearRange;
	//public float spread;                 // current spread of the gun
	public float baseFireRate;		//time between each shot
}

//freeze blast		