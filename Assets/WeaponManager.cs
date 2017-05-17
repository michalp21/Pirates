using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//At the beginning of each match, each player will be assigned an ID (Player1 or Player2) and
//WeaponManager will use these to make tags for all the weapons.

[RequireComponent (typeof (BaseShip))]
public class WeaponManager : MonoBehaviour {
	private int _maxWeapons;
	protected BaseShip _myShip;
	private int playerID;

	//dictionary with x,y position as key and GameObject as value
	public Dictionary<Vector2, GameObject> weaponDict = new Dictionary<Vector2, GameObject>();

	void Awake() {
		_myShip = GetComponent<BaseShip> ();
	}
	
	void Start () {
	}

	//Call this when adding the WeaponManager script
	void initWeaponManager (int id) {
		playerID = id;
		_maxWeapons = _myShip.WeaponSpace_x * _myShip.WeaponSpace_y;
		AssignTags();
	}

	private void AssignTags() {
		foreach(KeyValuePair<Vector2, GameObject> entry in weaponDict)
		{
			entry.Value.tag = playerID+"-Weapon"; //playerID needs to be implemented later
		}
	}

	//loop through, call fire() on all wpwns
	public void FireAll() {
		foreach(KeyValuePair<Vector2, GameObject> entry in weaponDict)
		{
			entry.Value.GetComponent<Gun>().Fire();
			// do something with entry.Value or entry.Key
		}
	}

	public void StartBoostAll(){
		//loop through, call boost() on all wpwns
		foreach(KeyValuePair<Vector2, GameObject> entry in weaponDict)
		{
			entry.Value.GetComponent<Gun>().StartBoost();
			// do something with entry.Value or entry.Key
		}
	}

	public void StopBoostAll(){
		//loop through, call boost() on all wpwns
		foreach(KeyValuePair<Vector2, GameObject> entry in weaponDict)
		{
			entry.Value.GetComponent<Gun>().StopBoost();
			// do something with entry.Value or entry.Key
		}
	}
}
