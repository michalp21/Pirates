using UnityEngine;
using System.Collections;

public class SimpleGunStats : WeaponStatsBase {
	
	// Use this for initialization
	void Start () {
		weaponName = "Simple Gun";
		rarity = 1;
		muzzlePoint = null;
		typeOfWeapon = WeaponType.FULLAUTO;
		usePooling = false;
		infiniteAmmo = true;
		maxLevel = 3;
	}

	public override void initWeaponStats(int currentLevel)
	{
		Debug.Assert(currentLevel <= maxLevel, "currentLevel exceeds maxLevel!");

		if (currentLevel == 0) {
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
		}
		else if (currentLevel == 1) {
			psurvive = ProjectileSurvive.AIR;
			damage.amount = 7;
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
		}
		else if (currentLevel == 2) {
			psurvive = ProjectileSurvive.AIR;
			damage.amount = 9;
			damage.type = DamageType.NORMAL;
			myResistances.normal = 1;
			myResistances.fire = 1;
			myResistances.ice = 1;
			myResistances.acid = 1;
			myResistances.electric = 1;
			myResistances.poison = 1;
			maxPenetration = 2;
			targetRange = 7f;
			disappearRange = 7;
			spread = 0;
			projectileSpeed = 3;
			projectileLifeTime = 5;
			baseFireRate = 1;
		}
	}
}
