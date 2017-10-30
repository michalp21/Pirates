using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct GridPosition
{
	public int row { get; set; }
	public int col { get; set; }

	public GridPosition(int r, int c) {
		row = r;
		col = c;
	}
}

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
	FULLAUTO
}

//USAGE: The GameObject will be in a prefab and the 
//stats without comments are NEW (ie not from NovaShot's original script)
//NEXT UP IS TARGETING
[RequireComponent (typeof (WeaponStatsBase))]
public class Gun : MonoBehaviour {
	public int currentLevel;
	public bool isSelf; //make into property when don't need inspector anymore? //as opposed to the opponent //isSelf will also be a variable in weaponstatsmanager
	public bool isSelected { get; set; }
	//public bool isSelected;
	public WeaponStatsBase weaponStats;
	public GridPosition gridPosition;

	//protected ProjectileInfo bulletInfo = new ProjectileInfo();

	//Below variables should not be public
	protected float fireRate;               // time betwen shots
	protected float nextFireTime = 0.0f;        // able to fire again on this frame
	protected Coroutine currentCoroutine;

    protected virtual void Start()
    {
		weaponStats = GetComponent<WeaponStatsBase>();
		fireRate = weaponStats.baseFireRate;
		currentCoroutine = null;
    }

	//Call initGun() after instantiating the GameObject with this Gun script attached
	protected void initGun(bool i) 
	{
		isSelf = i;
		//weaponStats = GetComponent<WeaponStatsBase>();
		//fireRate = weaponStats.baseFireRate;
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
	protected virtual GameObject FireOneShot()
	{
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
		fireRate = weaponStats.baseFireRate;
		Health myHealth = gameObject.GetComponent<Health>();
		if (currentCoroutine != null)
			StopCoroutine (currentCoroutine);
	}

    // set all bullet info from the gun's info
    // if you plan to be able to change weapon stats on the fly
    // call this function in the fire function (worst performance but always checkes gun stats before firing)
    // or Always call this just after altering a weapon's stats (best performance since its called once when it's needed)
    // default right now is it is called once in start
    /*protected void SetupBulletInfo()
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

		//if (gameObject.tag.Contains("Layer")){bulletInfo.enemyID = "Player2";}
		//else if (gameObject.tag.Contains("Player2")){bulletInfo.enemyID = "Player1";}
    }*/
}
