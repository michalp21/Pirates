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

	//public Dictionary<Vector3, Gun> weaponDict;
	private Column[] weaponGrid;
	public List<Gun> SelectedWeapons;
	private int effectiveLength;

	//TEMPORARY
	public Gun gun1;
	public Gun gun2;
	public Gun gun3;

	public float timeTakenDuringLerp;
	public float velocityOfLerp = 1.5f;
	private bool isLerping;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float timeStartedLerping;

	void Awake() {
		myShip = GetComponent<BaseShip> ();
		camera = Camera.main;
	}

	void Start () {
		// !! COLUMN MAJOR ORDER !! //
		weaponGrid = GetComponentsInChildren<Column> ();
		effectiveLength = weaponGrid.Length;

		foreach (Column col in weaponGrid) {
			col.initColumn ();
		}
		//TEMPORARY
		gun1.gridPosition.row = 0; gun1.gridPosition.col = 0;
		gun2.gridPosition.row = 0; gun2.gridPosition.col = 1;
		gun3.gridPosition.row = 0; gun3.gridPosition.col = 2;

		AddWeapons ();

		SelectedWeapons = new List<Gun> ();
		//healthBarSlider = GetComponentInChildren<Slider> ();
		healthBarSlider.value = healthBarSlider.maxValue = maxHealth;

		if (isSelf)
			selectorScript.initGenerateSelectorButtons ();
	}

	public void AddWeapons () {
		Gun[] childrenWeapons = GetComponentsInChildren<Gun> ();
		foreach (Gun weapon in childrenWeapons)
		{
			//weaponGrid.Add (weapon.gameObject.transform.position, weapon);
			maxHealth += weapon.GetComponent<WeaponStatsBase> ().health;
		}
	}

	public void RemoveWeapon(Gun g) {
		//weaponDict.Remove (v);
		weaponGrid[g.gridPosition.col].RemoveWeaponFromColumn(g.gridPosition.row);

		float distanceToMove = 0;
		if (weaponGrid [g.gridPosition.col].gunsRemaining == 0) {
			if (isSelf) {
				for (int i = g.gridPosition.col + 1; i < weaponGrid.Length; i++) {
					weaponGrid [i].KillColumn ();
					effectiveLength--;
				}
				distanceToMove = effectiveLength - g.gridPosition.col;
			} else {
				for (int i = g.gridPosition.col - 1; i >= 0; i--) {
					weaponGrid [i].KillColumn ();
					effectiveLength--;
				}
				distanceToMove = -1 - g.gridPosition.col;
			}
			MoveShip (distanceToMove);
		}


		if (SelectedWeapons.Contains (g))
			SelectedWeapons.Remove (g);
	}

	public void MoveShip(float dir) {
		//endPosition = new Vector3 (transform.position.x + dir, transform.position.y, transform.position.z);

		isLerping = true;
		timeStartedLerping = Time.time;
		timeTakenDuringLerp = Mathf.Abs(dir / velocityOfLerp);

		startPosition = transform.position;
		endPosition = transform.position + Vector3.right*dir;
	}

	public float? GetYPositionOfRowOnScreen (int k) {
		for (int l = 0; l < weaponGrid.Length; l++)
			if (weaponGrid [l][k] != null)
				return camera.WorldToScreenPoint ((Vector3) weaponGrid [l][k].transform.position).y;
		return null;
	}

	public float? GetXPositionOfColOnScreen (int l) {
		for (int k = 0; k < weaponGrid[l].GetLen(); k++)
			if (weaponGrid [l][k] != null)
				return camera.WorldToScreenPoint ((Vector3) weaponGrid [l][k].transform.position).x;
		return null;
	}

	public float? GetHeightOfRowOnScreen (int k) {
		for (int l = 0; l < weaponGrid.Length; l++)
			if (weaponGrid [l][k] != null) {
				Vector3 bottomBound = camera.WorldToScreenPoint (weaponGrid[l][k].GetComponent<BoxCollider>().bounds.center - new Vector3(0,weaponGrid[l][k].GetComponent<BoxCollider>().bounds.extents.y,0));
				Vector3 topBound = camera.WorldToScreenPoint (weaponGrid[l][k].GetComponent<BoxCollider>().bounds.center + new Vector3(0,weaponGrid[l][k].GetComponent<BoxCollider>().bounds.extents.y,0));
				return topBound.y - bottomBound.y;
			}
		return null;
	}

	public float? GetWidthOfColOnScreen (int l) {
		for (int k = 0; k < weaponGrid[l].GetLen(); k++)
			if (weaponGrid [l][k] != null) {
				Vector3 leftBound = camera.WorldToScreenPoint (weaponGrid[l][k].GetComponent<BoxCollider>().bounds.center - new Vector3(weaponGrid[l][k].GetComponent<BoxCollider>().bounds.extents.x,0,0));
				Vector3 rightBound = camera.WorldToScreenPoint (weaponGrid[l][k].GetComponent<BoxCollider>().bounds.center + new Vector3(weaponGrid[l][k].GetComponent<BoxCollider>().bounds.extents.x,0,0));
				return rightBound.x - leftBound.x;
			}
		return null;
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
		for (int l = 0; l < weaponGrid.Length; l++) {
			for (int k = 0; k < weaponGrid[l].GetLen(); k++) {
				if (weaponGrid [l][k] == null)
					continue;
				if (weaponGrid [l][k].GetComponent<WeaponStatsBase> ().typeOfWeapon == WeaponType.FULLAUTO)
				{
					weaponGrid [l][k].Fire();
				}
				else
				{
					Debug.Log("Chaw haw haw, it's not FULLAUTO");
				}
			}
		}
	}

	public bool ToggleSelectRow(int k, bool selected) {
		if (!selected) {
			for (int l = 0; l < weaponGrid.Length; l++)
				if (weaponGrid [l][k] != null && !weaponGrid [l][k].isSelected) {
					SelectedWeapons.Add (weaponGrid [l][k]);
					weaponGrid [l][k].isSelected = true;
					weaponGrid [l][k].GetComponent<SpriteRenderer> ().enabled = true;
				}
			return true;
		} else {
			for (int l = 0; l < weaponGrid.Length; l++)
				if (weaponGrid [l][k] != null) {
					SelectedWeapons.Remove (weaponGrid [l][k]);
					weaponGrid [l][k].isSelected = false;
					weaponGrid [l][k].GetComponent<SpriteRenderer> ().enabled = false;
				}
			return false;
		}
	}

	public bool ToggleSelectColumn(int l, bool selected) {
		if (!selected) {
			for (int k = 0; k < weaponGrid[l].GetLen(); k++)
				if (weaponGrid [l][k] != null && !weaponGrid [l][k].isSelected) {
					SelectedWeapons.Add (weaponGrid [l][k]);
					weaponGrid [l][k].isSelected = true;
					weaponGrid [l][k].GetComponent<SpriteRenderer> ().enabled = true;
				}
			return true;
		} else {
			for (int k = 0; k < weaponGrid[l].GetLen(); k++)
				if (weaponGrid [l][k] != null) {
					SelectedWeapons.Remove (weaponGrid [l][k]);
					weaponGrid [l][k].isSelected = false;
					weaponGrid [l][k].GetComponent<SpriteRenderer> ().enabled = false;
				}
			return false;
		}
	}

	//when other selections besides ToggleSelectRow happen to completely select a row
	public bool CheckIfRowIsSelected(int k) {
		for (int l = 0; l < weaponGrid.Length; l++) {
			if (weaponGrid [l][k] != null && weaponGrid [l][k].isSelected == false)
				return false;
		}
		return true;
	}

	//when other selections besides ToggleSelectColumn happen to completely select a column
	public bool CheckIfColumnIsSelected(int l) {
		for (int k = 0; k < weaponGrid[l].GetLen(); k++) {
			if (weaponGrid [l][k] != null && weaponGrid [l][k].isSelected == false)
				return false;
		}
		return true;
	}

	public void StartBoostOnSelected() {
		//loop through, call boost() on all wpwns
		/*foreach(KeyValuePair<Vector3, Gun> entry in weaponDict)
		{
			entry.Value.StartBoost();
			// do something with entry.Value or entry.Key
		}*/
		foreach (Gun weapon in SelectedWeapons)
			if (weapon != null)
				weapon.StartBoost ();
	}

	public void StopBoostOnSelected() {
		//loop through, call boost() on all wpwns
		/*foreach(KeyValuePair<Vector3, Gun> entry in weaponDict)
		{
			entry.Value.StopBoost();
			// do something with entry.Value or entry.Key
		}*/
		foreach (Gun weapon in SelectedWeapons)
			if (weapon != null)
				weapon.StopBoost ();
	}

	public int GetNumRowsInGrid() {
		return weaponGrid[0].GetLen();
	}

	public int GetNumColumnsInGrid() {
		return weaponGrid.Length;
	}

	public Gun GetWeaponInGrid(int k, int l) {
		return weaponGrid [l][k];
	}

	void Update () {
		FireAll ();
	}

	void FixedUpdate () {
		if (isLerping) {
			float timeSinceStarted = Time.time - timeStartedLerping;
			float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
			transform.position = Vector3.Lerp (startPosition, endPosition, percentageComplete);
			Debug.Log (transform.position);

			if (percentageComplete >= 1.0f)
				isLerping = false;
		}
	}
}
