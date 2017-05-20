using UnityEngine;
using System.Collections;

//Will handle boost, weaponSelection, panning, etc
public class PlayerInput : MonoBehaviour {

    public Gun myGun;

	// Use this for initialization
	void Start () {
        myGun = GetComponentInChildren<Gun>(); // grab a gun script from player on start
	}
	
	// Now used for boost
	// CHANGE LATER SO IT DOESNT FIRE BASED ON BUTTON PRESS
	void Update () {
        if (myGun)
        {
            if (myGun.weaponStats.typeOfWeapon == WeaponType.FULLAUTO)
            {
				//Can use GetButton or GetKey
                if (Input.GetKey(KeyCode.Space))
                {
					myGun.StartBoost();
                }
            }
            else
            {
				Debug.Log("Chaw haw haw, it's not FULLAUTO");
            }
        }
	}
}
