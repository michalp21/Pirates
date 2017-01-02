using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Damage
{
	public int amount; // how much damage
	public DamageType type; //what type of damage
}

public enum DamageType
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
	FULLAUTO
}

//USAGE: The GameObject will be in a prefab and the 
//stats without comments are NEW (ie not from NovaShot's original script)
//NEXT UP IS TARGETING
[RequireComponent (typeof (WeaponStatsBase))]
public class Gun : MonoBehaviour {
	public int currentLevel;
	public string playerID;
	public WeaponStatsBase weaponStats;

	public string weaponName;                         // Name of this weapon
	public int rarity;
	public Transform muzzlePoint;        // the muzzle point for this gun, where you want bullets to be spawned
	public WeaponType typeOfWeapon;             // type of weapon, used to determine how the trigger acts
	public bool usePooling;             // do we want to use object pooling or instantiation
	public bool infiniteAmmo;		//whether there is infinite ammo
	public int maxLevel;
	public ProjectileSurvive psurvive;
	
	public Damage damage;        // the damage and type of damage this gun does
	public int maxPenetration;              // maximum amount of hits detected before the bullet is destroyed
	public int disappearRange;
	public float spread;                 // current spread of the gun
	public float projectileSpeed;      // speed that projectile flies at
	public float projectileLifeTime;     // how long before the projectile is considered gone and recycleable
	public float baseFireRate;

	//Below variables should not be public
	protected float fireRate;               // time betwen shots
	protected float nextFireTime = 0.0f;        // able to fire again on this frame
    protected ProjectileInfo bulletInfo = new ProjectileInfo(); // all info about gun that's sent to each projectile

	//wont use this because I need an init function with parameters
	//and I'm too lazy to make it work with Start()
    protected virtual void Start()
    {

    }

	//Call initGun() after instantiating the GameObject with this Gun script attached
	protected void initGun(int cl, string id) //maybe add parameter for WeaponStatsBase component
	{
		currentLevel = cl;
		playerID = id; //"Player1" or "Player2"
		weaponStats = GetComponent<WeaponStatsBase>();
		weaponStats.initWeaponStats(currentLevel);

		weaponName = weaponStats.weaponName;
		rarity = weaponStats.rarity;
		muzzlePoint = weaponStats.muzzlePoint;
		typeOfWeapon = weaponStats.typeOfWeapon;
		usePooling = weaponStats.usePooling;
		infiniteAmmo = weaponStats.infiniteAmmo;
		maxLevel = weaponStats.maxLevel;

		psurvive = weaponStats.psurvive;
		damage = weaponStats.damage;
		maxPenetration = weaponStats.maxPenetration;
		disappearRange = weaponStats.disappearRange;
		spread = weaponStats.spread;
		projectileSpeed = weaponStats.projectileSpeed;
		projectileLifeTime = weaponStats.projectileLifeTime;
		baseFireRate = weaponStats.baseFireRate;

		fireRate = weaponStats.baseFireRate;
	}

	public void LevelUp()
	{
		currentLevel++;
	}

    // all guns handle firing a bit different so give it a blank function that each gun can override
    public virtual void Fire()
    {

    }

	// everything fires a single round the same
	protected virtual void FireOneShot()
	{
		
	}

	public virtual void StartBoost ()
	{

	}
	
	public virtual void StopBoost ()
	{

	}

    // set all bullet info from the gun's info
    // if you plan to be able to change weapon stats on the fly
    // call this function in the fire function (worst performance but always checkes gun stats before firing)
    // or Always call this just after altering a weapon's stats (best performance since its called once when it's needed)
    // default right now is it is called once in start
    protected void SetupBulletInfo()
    {
        bulletInfo.owner = transform.root.gameObject;   // the Owner of this weapon (GameObject) <- use this for scoreboard and who killed who
		bulletInfo.name = weaponName;                         // Name of this weapon  <- for keeping track of weapon kills / whose killed by what
		bulletInfo.damage.amount = damage.amount;       // amount of damage
		bulletInfo.damage.type = damage.type;           // type of damage
		bulletInfo.maxPenetration = maxPenetration;     // max hits
		bulletInfo.spread = spread;                     // current weapon spread value
		bulletInfo.speed = projectileSpeed;             // projectile speed
		bulletInfo.projectileLifeTime = projectileLifeTime; // how long till this bullet just goes away
		bulletInfo.usePool = usePooling;                // do we use object pooling

		if (gameObject.tag.Contains("Player1")){bulletInfo.enemyID = "Player2";}
		else if (gameObject.tag.Contains("Player2")){bulletInfo.enemyID = "Player1";}
    }
}
