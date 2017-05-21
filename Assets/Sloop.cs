using UnityEngine;
using System.Collections;

public class Sloop : BaseShip {

	void Start() {
		weaponSpace_cols = 3;
		weaponSpace_rows = 1;
		peopleSpace = 5;
		shipName = "Sloop";
		rarity = 1;
	}

	public override void fireMainWeapon(){
		Debug.Log ("Sloop: SLEM SLEM");
	}
}
