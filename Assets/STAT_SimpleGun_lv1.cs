using UnityEngine;
using System.Collections;

//POSSIBLY SPLIT THIS SCRIPT INTO SMALLER SCRIPTS FOR EACH LEVEL FOR USE WITH PREFABS
public class STAT_SimpleGun_lv1 : WeaponStatsBase {
	
	// Use this for initialization
	void Awake () {
		currentLevel = 1;
		weaponName = "Simple Gun";
		rarity = 1;
		//muzzlePoint = null;
		typeOfWeapon = WeaponType.FULLAUTO;
		usePooling = false;
		infiniteAmmo = true;
		maxLevel = 3;

		psurvive = ProjectileSurvive.AIR;
		damage.amount = 5;
		damage.type = DamageType.NORMAL;
		myResistances.normal = 1;
		myResistances.fire = 1;
		myResistances.ice = 1;
		myResistances.acid = 1;
		myResistances.electric = 1;
		myResistances.poison = 1;
		maxPenetration = 1;
		targetRange = 7f;
		disappearRange = 7;
		spread = 0;
		projectileSpeed = 3;
		projectileLifeTime = 5;
		baseFireRate = 1;

		health = 100;
	}
}
