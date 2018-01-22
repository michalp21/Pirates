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

        gun1.GetComponent<ShipAttack>().gridPosition.row = 0; gun1.GetComponent<ShipAttack>().gridPosition.col = 0;
        gun2.GetComponent<ShipAttack>().gridPosition.row = 0; gun2.GetComponent<ShipAttack>().gridPosition.col = 1;
        gun3.GetComponent<ShipAttack>().gridPosition.row = 0; gun3.GetComponent<ShipAttack>().gridPosition.col = 2;

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
			maxHealth += weapon.GetComponent<WeaponStatsBase> ().health;
		}
	}

    public void RemoveWeapon(ShipAttack sa) {
		float distanceToMove = 0;
		if (weaponGrid [sa.gridPosition.col].gunsRemaining > 0) { //don't want to kill a dead column
			weaponGrid [sa.gridPosition.col].RemoveWeaponFromColumn (sa.gridPosition.row);
			effectiveLength--; distanceToMove++;
		}

		if (weaponGrid [sa.gridPosition.col].gunsRemaining == 0) {
			if (isSelf) {
				for (int l = sa.gridPosition.col + 1; l < weaponGrid.Length; l++) {
					if (weaponGrid [l].gunsRemaining > 0) { //don't want to kill a dead column
						weaponGrid [l].KillColumn ();
						effectiveLength--; distanceToMove++;
					}
				}
				MoveShip (distanceToMove);
			} else {
				for (int l = sa.gridPosition.col - 1; l >= 0; l--) {
					if (weaponGrid [l].gunsRemaining > 0) { //don't want to kill a dead column
						weaponGrid [l].KillColumn ();
						effectiveLength--; distanceToMove++;
					}
				}
				MoveShip (-distanceToMove);
			}
		}

        if (SelectedWeapons.Contains (sa.GetComponent<Gun>()))
			SelectedWeapons.Remove (sa.GetComponent<Gun>());
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

	public void TakeDamage(int damage) {
		healthBarSlider.value -= damage;
	}

	//loop through, call fire() on all wpwns
	public void FireAll() {
		for (int l = 0; l < weaponGrid.Length; l++) {
			for (int k = 0; k < weaponGrid[l].GetLen(); k++) {
				if (weaponGrid [l][k] == null)
					continue;
				weaponGrid [l][k].Attack();
			}
		}
	}

	public bool ToggleSelectIndividual(int k, int l, bool s) {
		
		if (s) {
            if (weaponGrid [l][k] != null && !weaponGrid [l][k].GetComponent<ShipAttack>().isSelected) {
				SelectedWeapons.Add (weaponGrid [l] [k]);
                weaponGrid [l][k].GetComponent<ShipAttack>().isSelected = s;
				weaponGrid [l][k].GetComponent<SpriteRenderer> ().enabled = s;
			}
			return true;
		} else {
			if (weaponGrid [l][k] != null) {
				SelectedWeapons.Remove (weaponGrid [l] [k]);
                weaponGrid [l][k].GetComponent<ShipAttack>().isSelected = false;
				weaponGrid [l][k].GetComponent<SpriteRenderer> ().enabled = false;
			}
			return false;
		}

	}

	public bool ToggleSelectRow(int k, bool selected) {
		if (!selected) {
			for (int l = 0; l < weaponGrid.Length; l++)
				ToggleSelectIndividual (k, l, true);
			return true;
		} else {
			for (int l = 0; l < weaponGrid.Length; l++)
				ToggleSelectIndividual (k, l, false);
			return false;
		}
	}

	public bool ToggleSelectColumn(int l, bool selected) {
		if (!selected) {
			for (int k = 0; k < weaponGrid[l].GetLen(); k++)
				ToggleSelectIndividual (k, l, true);
			return true;
		} else {
			for (int k = 0; k < weaponGrid[l].GetLen(); k++)
				ToggleSelectIndividual (k, l, false);
			return false;
		}
	}

	//when other selections besides ToggleSelectRow happen to completely select a row
	public bool CheckIfRowIsSelected(int k) {
		for (int l = 0; l < weaponGrid.Length; l++) {
            if (weaponGrid [l][k] != null && weaponGrid [l][k].GetComponent<ShipAttack>().isSelected == false)
				return false;
		}
		return true;
	}

	//when other selections besides ToggleSelectColumn happen to completely select a column
	public bool CheckIfColumnIsSelected(int l) {
		for (int k = 0; k < weaponGrid[l].GetLen(); k++) {
            if (weaponGrid [l][k] != null && weaponGrid [l][k].GetComponent<ShipAttack>().isSelected == false)
				return false;
		}
		return true;
	}

	public void StartBoostSelected() {
		foreach (Gun weapon in SelectedWeapons)
			if (weapon != null) {
				weapon.StartBoost ();
			}
	}

	public void StopBoostSelected() {
		foreach (Gun weapon in SelectedWeapons)
			if (weapon != null)
				weapon.StopBoost ();
	}

	public void TargetSelected(Gun newTarget) {
		foreach (Gun weapon in SelectedWeapons)
			if (weapon != null) {
				weapon.GetComponentInChildren<Target> ().manualTarget (newTarget);
			}
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

			if (percentageComplete >= 1.0f)
				isLerping = false;
		}
	}
}
