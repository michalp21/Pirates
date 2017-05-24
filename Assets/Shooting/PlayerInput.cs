using UnityEngine;
using System.Collections;

//THIS IS JUST FOR TESTING VARIOUS THINGS
public class PlayerInput : MonoBehaviour {

    public Gun[] myGun;
	public Camera camera;

	void Start () {
        myGun = GetComponentsInChildren<Gun>();
		camera = Camera.main;
	}

	void Update () {
		/*foreach (Gun weapon in myGun) {
			if (weapon) {
				if (weapon.weaponStats.typeOfWeapon == WeaponType.FULLAUTO) {
					//Can use GetButton or GetKey
					if (Input.GetKey (KeyCode.Space)) {
						Debug.Log ("asdf");
						weapon.transform.position = new Vector3(weapon.transform.position.x + .1f, weapon.transform.position.y + .1f, weapon.transform.position.z);
					}
				} else {
					Debug.Log ("Chaw haw haw, it's not FULLAUTO");
				}
			}
		}*/
		if(Input.GetKey(KeyCode.RightArrow))
		{
			camera.transform.Translate(new Vector3(-3 * Time.deltaTime,0,0));
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			camera.transform.Translate(new Vector3(3 * Time.deltaTime,0,0));
		}
		if(Input.GetKey(KeyCode.DownArrow))
		{
			camera.transform.Translate(new Vector3(0,3 * Time.deltaTime,0));
		}
		if(Input.GetKey(KeyCode.UpArrow))
		{
			camera.transform.Translate(new Vector3(0,-3 * Time.deltaTime,0));
		}
		//camera.transform.Translate(0,0,Input.GetAxis("Mouse ScrollWheel") * 10);
		camera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * 8;
		//Debug.Log(camera.orthographicSize);
	}
}
