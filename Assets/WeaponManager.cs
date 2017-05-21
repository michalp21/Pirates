using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour {
	private int maxWeapons;
	private BaseShip myShip;
	private int maxHealth;
	//private int playerID;

	public Slider healthBarSlider;

	private Gun[] childrenWeapons;
	//public Dictionary<Vector3, Gun> weaponDict;
	public Gun[,] weaponGrid;

	//TEMPORARY
	public Gun gun1;
	public Gun gun2;

	void Awake() {
		myShip = GetComponent<BaseShip> ();
	}

	void Start () {
		weaponGrid = new Gun[myShip.weaponSpace_rows, myShip.weaponSpace_cols];
		//TEMPORARY
		weaponGrid[0,0] = gun1;
		weaponGrid[0,1] = gun2;
		weaponGrid[0,2] = null;

		AddWeapons ();
		//healthBarSlider = GetComponentInChildren<Slider> ();
		healthBarSlider.value = healthBarSlider.maxValue = maxHealth;
	}

	public void AddWeapons () {
		childrenWeapons = GetComponentsInChildren<Gun> ();
		foreach (Gun weapon in childrenWeapons)
		{
			//weaponGrid.Add (weapon.gameObject.transform.position, weapon);
			maxHealth += weapon.GetComponent<WeaponStatsBase> ().health;
			Debug.Log (maxHealth);
		}
	}

	public void RemoveWeapon(Gun g) {
		//weaponDict.Remove (v);
		weaponGrid[g.gridPosition.row, g.gridPosition.col] = null;
	}

	//Call this when adding the WeaponManager script
	/*void initWeaponManager (int id) {
		playerID = id;
		_maxWeapons = _myShip.WeaponSpace_x * _myShip.WeaponSpace_y;
		AssignTags();
	}

	private void AssignTags() {
		foreach(KeyValuePair<Vector2, GameObject> entry in weaponDict)
		{
			entry.Value.tag = playerID+"-Weapon"; //playerID needs to be implemented later
		}
	}*/

	public void TakeDamage(int damage) {
		Debug.Log ("Taking damage");
		healthBarSlider.value -= damage;
	}



	//loop through, call fire() on all wpwns
	public void FireAll() {
		/*foreach(KeyValuePair<Vector3, Gun> weapon in weaponDict)
		{
			//weapon.Value.Fire();
			// do something with entry.Value or entry.Key

			if (weapon.Value.GetComponent<WeaponStatsBase> ().typeOfWeapon == WeaponType.FULLAUTO)
			{
				weapon.Value.Fire();
			}
			else
			{
				Debug.Log("Chaw haw haw, it's not FULLAUTO");
			}
		}*/
		for (int k = 0; k < weaponGrid.GetLength (0); k++) {
			for (int l = 0; l < weaponGrid.GetLength (1); l++) {
				if (weaponGrid [k, l] == null)
					continue;
				if (weaponGrid[k,l].GetComponent<WeaponStatsBase> ().typeOfWeapon == WeaponType.FULLAUTO)
				{
					weaponGrid[k,l].Fire();
				}
				else
				{
					Debug.Log("Chaw haw haw, it's not FULLAUTO");
				}
			}
		}
	}

	public void Select() {

	}

	public void DeSelect() {

	}

	public void StartBoostAll() {
		//loop through, call boost() on all wpwns
		/*foreach(KeyValuePair<Vector3, Gun> entry in weaponDict)
		{
			entry.Value.StartBoost();
			// do something with entry.Value or entry.Key
		}*/
		for (int k = 0; k < weaponGrid.GetLength (0); k++)
			for (int l = 0; l < weaponGrid.GetLength (1); l++)
				weaponGrid [k, l].StartBoost ();
	}

	public void StopBoostAll() {
		//loop through, call boost() on all wpwns
		/*foreach(KeyValuePair<Vector3, Gun> entry in weaponDict)
		{
			entry.Value.StopBoost();
			// do something with entry.Value or entry.Key
		}*/
		for (int k = 0; k < weaponGrid.GetLength (0); k++)
			for (int l = 0; l < weaponGrid.GetLength (1); l++)
				weaponGrid [k, l].StopBoost ();
	}

	public Gun GetWeaponInGrid(int k, int l) {
		return weaponGrid [k, l];
	}

	void Update () {
		FireAll ();
	}
}
