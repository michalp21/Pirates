using UnityEngine;
using System.Collections;

//Separate weapon stuff from projectile stuff into respective classes. Get rid of the current bestowing/initializing.

//Consider making a STRUCT
public abstract class WeaponStatsBase : MonoBehaviour
{
	public int currentLevel;
	public string weaponName;                         // Name of this weapon
	public int rarity;
	public WeaponType weaponType;             // type of weapon, used to determine how the trigger acts
	public Resistance myResistances = new Resistance();
	public float baseRate;		//time between each shot

	public bool usePooling;             // do we want to use object pooling or instantiation
	//public bool infiniteAmmo;		//whether there is infinite ammo
	//public int maxLevel;
	public int health;

	public Transform muzzlePoint;        // the muzzle point for this gun, where you want bullets to be spawned
	public float targetRange;
	public int disappearRange;			//CHANGE: to trigger boxes around arena to destroy/pool
	//public float spread;                 // current spread of the gun

	/*public ProjectileSurvive psurvive;
	public int damage;
	public Element damageType;*/
}

//freeze blast		