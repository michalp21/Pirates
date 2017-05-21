using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateSelectorButtons : MonoBehaviour {

	public WeaponManager weaponManager;
	public Button verticalButton; //inspector
	public Button horizontalButton; //inspector

	public GameObject verticalSelector; //inspector
	public GameObject horizontalSelector; //inspector

	private int numCols;
	private int numRows;

	// Use this for initialization
	void Start () {
		numRows = weaponManager.weaponGrid.GetLength(0);
		numCols = weaponManager.weaponGrid.GetLength(1);

		initVerticalSelectors (numRows);
		initHorizontalSelectors (numCols);
	}

	public void initVerticalSelectors (int numRows) {
		for (int k = 0; k < numCols; k++) {
			Transform someGun = weaponManager.GetWeaponInGrid (k, 0).gameObject.transform;
			Button button = Instantiate (horizontalButton, new Vector3(0, someGun.position.y, -1.1f) , Quaternion.identity);
			button.transform.SetParent (verticalSelector.transform);
		}
	}

	public void initHorizontalSelectors (int numCols) {
		for (int l = 0; l < numCols; l++) {
			Transform someGun = weaponManager.GetWeaponInGrid (0, l).gameObject.transform;
			Button button = Instantiate (horizontalButton, new Vector3(someGun.position.x, 0, -1.1f) , Quaternion.identity);
			button.transform.SetParent (horizontalSelector.transform);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
