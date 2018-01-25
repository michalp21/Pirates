using UnityEngine;
using System.Collections;

//POSSIBLY SPLIT THIS SCRIPT INTO SMALLER SCRIPTS FOR EACH LEVEL FOR USE WITH PREFABS
public class STAT_Bomber_lv1 : WeaponStatsBase {
	
	// Use this for initialization
	void Awake () {
		currentLevel = 1;
		weaponName = "Simple Gun";
		rarity = 1;
		//muzzlePoint = null;
		weaponType = WeaponType.NONCHILDING;
		usePooling = false;
		health = 100;
		myResistances.normal = 1;
		myResistances.fire = 1;
		myResistances.ice = 1;
		myResistances.acid = 1;
		myResistances.electric = 1;
		myResistances.poison = 1;
		targetRange = 7f;
		disappearRange = 7;
		baseRate = 2;
	}
}
