using UnityEngine;
using System.Collections;

public class Sloop : BaseShip {

	void Start(){
		WeaponSpace_x = 3;
		WeaponSpace_y = 1;
		PeopleSpace = 5;
		ShipName = "Sloop";
		Rarity = 1;
	}

	public override void fireMainWeapon(){
		Debug.Log ("Sloop: SLEM SLEM");
	}
}
