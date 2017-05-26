using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour {
	private int maxWeapons;
	private BaseShip myShip;
	private int maxHealth;
	//private int playerID;

	public Camera camera;
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
		camera = Camera.main;
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

	public float? GetYPositionOfRowOnScreen (int k) {
		for (int l = 0; l < weaponGrid.GetLength (1); l++)
			if (weaponGrid [k, l] != null)
				return camera.WorldToScreenPoint ((Vector3) weaponGrid [k, l].transform.position).y;
		return null;
	}

	public float? GetXPositionOfColOnScreen (int l) {
		for (int k = 0; k < weaponGrid.GetLength (0); k++)
			if (weaponGrid [k, l] != null)
				return camera.WorldToScreenPoint ((Vector3) weaponGrid [k, l].transform.position).x;
		return null;
	}

	public float? GetHeightOfRowOnScreen (int k) {
		for (int l = 0; l < weaponGrid.GetLength (1); l++)
			if (weaponGrid [k, l] != null) {
				Vector3 bottomBound = camera.WorldToScreenPoint (weaponGrid[k, l].GetComponent<BoxCollider>().bounds.center - new Vector3(0,weaponGrid[k, l].GetComponent<BoxCollider>().bounds.extents.y,0));
				Vector3 topBound = camera.WorldToScreenPoint (weaponGrid[k, l].GetComponent<BoxCollider>().bounds.center + new Vector3(0,weaponGrid[k, l].GetComponent<BoxCollider>().bounds.extents.y,0));
				return topBound.y - bottomBound.y;
			}
		return null;
	}

	public float? GetWidthOfColOnScreen (int l) {
		for (int k = 0; k < weaponGrid.GetLength (0); k++)
			if (weaponGrid [k, l] != null) {
				Vector3 leftBound = camera.WorldToScreenPoint (weaponGrid[k, l].GetComponent<BoxCollider>().bounds.center - new Vector3(weaponGrid[k, l].GetComponent<BoxCollider>().bounds.extents.x,0,0));
				Vector3 rightBound = camera.WorldToScreenPoint (weaponGrid[k, l].GetComponent<BoxCollider>().bounds.center + new Vector3(weaponGrid[k, l].GetComponent<BoxCollider>().bounds.extents.x,0,0));
				return rightBound.x - leftBound.x;
			}
		return null;
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

	public void SelectRow(int k) {
		for (int l = 0; l < weaponGrid.GetLength (1); l++)
			if (weaponGrid [k, l] != null)
				SelectedWeapons.Add (weaponGrid [k, l]);
	}

	public void DeselectRow(int k) {
		for (int l = 0; l < weaponGrid.GetLength (1); l++)
			if (weaponGrid [k, l] != null)
				SelectedWeapons.Remove (weaponGrid [k, l]);
	}

	public void SelectColumn(int l) {
		for (int k = 0; k < weaponGrid.GetLength (0); k++)
			if (weaponGrid [k, l] != null)
				SelectedWeapons.Add (weaponGrid [k, l]);
	}

	public void DeselectColumn(int l) {
		for (int k = 0; k < weaponGrid.GetLength (0); k++)
			if (weaponGrid [k, l] != null)
				SelectedWeapons.Remove (weaponGrid [k, l]);
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
				if (weaponGrid [k, l] != null)
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
				if (weaponGrid [k, l] != null)
					weaponGrid [k, l].StopBoost ();
	}

	public int GetNumRowsInGrid() {
		return weaponGrid.GetLength(0);
	}

	public int GetNumColumnsInGrid() {
		return weaponGrid.GetLength(1);
	}

	public Gun GetWeaponInGrid(int k, int l) {
		//Debug.Log ("Get weapon in Grid: " + k + "," + l);
		return weaponGrid [k, l];
	}

	void Update () {
		FireAll ();
	}
}
