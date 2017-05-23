using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour {
	private int maxWeapons;
	private BaseShip myShip;
	private int maxHealth;
	//private int playerID;

	public Slider healthBarSlider; //inspector
	public GenerateSelectorButtons selectorScript; //inspector
	public bool isSelf; //TEMPORARY 

	private Gun[] childrenWeapons;
	//public Dictionary<Vector3, Gun> weaponDict;
	private Gun[,] weaponGrid;
	public List<Gun> SelectedWeapons; 

	//TEMPORARY
	public Gun gun1;
	public Gun gun2;
	public Gun gun3;

	void Awake() {
		myShip = GetComponent<BaseShip> ();
	}

	void Start () {
		//Debug.Log (myShip.weaponSpace_rows + " " + myShip.weaponSpace_cols);
		weaponGrid = new Gun[myShip.weaponSpace_rows, myShip.weaponSpace_cols];
		//TEMPORARY
		weaponGrid[0,0] = gun1;
		weaponGrid[0,1] = gun2;
		weaponGrid[0,2] = gun3;

		AddWeapons ();

		SelectedWeapons = new List<Gun> ();
		//healthBarSlider = GetComponentInChildren<Slider> ();
		healthBarSlider.value = healthBarSlider.maxValue = maxHealth;

		if (isSelf)
			selectorScript.initGenerateSelectorButtons ();
	}

	public void AddWeapons () {
		childrenWeapons = GetComponentsInChildren<Gun> ();
		foreach (Gun weapon in childrenWeapons)
		{
			//weaponGrid.Add (weapon.gameObject.transform.position, weapon);
			maxHealth += weapon.GetComponent<WeaponStatsBase> ().health;
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

	public void SelectRow(GridPosition gp) {
		for (int l = 0; l < weaponGrid.GetLength (1); l++)
			SelectedWeapons.Add (weaponGrid [gp.row, l]);
		foreach(Gun str in SelectedWeapons)
			Debug.Log (str);
	}

	public void DeselectRow(GridPosition gp) {
		for (int l = 0; l < weaponGrid.GetLength (1); l++)
			SelectedWeapons.Remove (weaponGrid [gp.row, l]);
		Debug.Log ("REMOVING");
		foreach(Gun str in SelectedWeapons)
			Debug.Log (str);
	}

	public void SelectColumn(GridPosition gp) {
		for (int k = 0; k < weaponGrid.GetLength (0); k++)
			SelectedWeapons.Add (weaponGrid [k, gp.col]);
	}

	public void DeselectColumn(GridPosition gp) {
		for (int k = 0; k < weaponGrid.GetLength (0); k++)
			SelectedWeapons.Remove (weaponGrid [k, gp.col]);
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

	public int GetNumRowsInGrid() {
		return weaponGrid.GetLength(0);
	}

	public int GetNumColumnsInGrid() {
		return weaponGrid.GetLength(1);
	}

	public Gun GetWeaponInGrid(int k, int l) {
		Debug.Log ("Get weapon in Grid: " + k + "," + l);
		return weaponGrid [k, l];
	}

	void Update () {
		FireAll ();
	}
}
