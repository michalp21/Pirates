using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    public Gun myGun;

	// Use this for initialization
	void Start () {
        myGun = GetComponentInChildren<Gun>(); // grab a gun script from player on start
	}
	
	// Update is called once per frame
	// CHANGE LATER SO IT DOESNT FIRE BASED ON BUTTON PRESS
	void Update () {
        if (myGun)
        {	
			Debug.Log (myGun.weaponStats);
            if (myGun.weaponStats.typeOfWeapon == WeaponType.FULLAUTO)
            {
				//Can use GetButton or GetKey
                if (Input.GetKey(KeyCode.Space))
                {
                    myGun.Fire();
                }
            }
            else
            {
				Debug.Log("Chaw haw haw, it's not FULLAUTO");
            }
        }
	}
}
