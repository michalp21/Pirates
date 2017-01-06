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
	protected void initGun(string id) //maybe add parameter for WeaponStatsBase component
	{
		playerID = id; //"Player1" or "Player2"
		weaponStats = GetComponent<WeaponStatsBase>();
		fireRate = weaponStats.baseFireRate;
	}

	public void LevelUp()
	{
		//currentLevel++; //no longer sufficient
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
		//include disappear range?
        bulletInfo.owner = transform.root.gameObject;   // the Owner of this weapon (GameObject) <- use this for scoreboard and who killed who
		bulletInfo.name = weaponStats.weaponName;                         // Name of this weapon  <- for keeping track of weapon kills / whose killed by what
		bulletInfo.damage.amount = weaponStats.damage.amount;       // amount of damage
		bulletInfo.damage.type = weaponStats.damage.type;           // type of damage
		bulletInfo.maxPenetration = weaponStats.maxPenetration;     // max hits
		bulletInfo.spread = weaponStats.spread;                     // current weapon spread value
		bulletInfo.speed = weaponStats.projectileSpeed;             // projectile speed
		bulletInfo.projectileLifeTime = weaponStats.projectileLifeTime; // how long till this bullet just goes away
		bulletInfo.usePool = weaponStats.usePooling;                // do we use object pooling

		if (gameObject.tag.Contains("Player1")){bulletInfo.enemyID = "Player2";}
		else if (gameObject.tag.Contains("Player2")){bulletInfo.enemyID = "Player1";}
    }
}
