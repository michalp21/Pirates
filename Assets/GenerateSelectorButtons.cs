﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateSelectorButtons : MonoBehaviour {

	public WeaponManager weaponManager;
	public Button verticalButton; //inspector
	public Button horizontalButton; //inspector

	//These are actually the masks, not exactly the "VerticalSelector" and "HorizontalSelector" themselves
	public GameObject verticalSelector; //inspector
	public GameObject horizontalSelector; //inspector

	public Camera camera;

	private int numCols;
	private int numRows;

	void Start () {
		
	}

	public void initGenerateSelectorButtons () {
		camera = Camera.main;

		numRows = weaponManager.GetNumRowsInGrid();
		numCols = weaponManager.GetNumColumnsInGrid();

		instantiateVerticalSelectors ();
		instantiateHorizontalSelectors ();
	}

	public void instantiateVerticalSelectors () {
		for (int k = 0; k < numRows; k++) {
			GameObject someGun = weaponManager.GetWeaponInGrid (k, 0).gameObject;
			Button button = Instantiate (verticalButton, verticalSelector.transform);
			//Button button = Instantiate (verticalButton, new Vector3(0, someGun.transform.position.y, -1.1f) , Quaternion.identity);
			//button.transform.SetParent (verticalSelector.transform);
			Vector3 screenPos = camera.WorldToScreenPoint(someGun.transform.position);
			button.transform.position = new Vector3(button.transform.position.x, screenPos.y, button.transform.position.z);
			button.GetComponent<VerticalSelectorPointerListener> ().initSelector (k, screenPos.y);
		}
	}

	public void instantiateHorizontalSelectors () {
		for (int l = 0; l < numCols; l++) {
			GameObject someGun = weaponManager.GetWeaponInGrid (0, l).gameObject;
			Button button = Instantiate (horizontalButton, horizontalSelector.transform);
			//Button button = Instantiate (horizontalButton, new Vector3(someGun.transform.position.x, 0, -1.1f), Quaternion.identity);
			//button.transform.SetParent (horizontalSelector.transform);
			Vector3 screenPos = camera.WorldToScreenPoint(someGun.transform.position);
			button.transform.position = new Vector3(screenPos.x, button.transform.position.y, button.transform.position.z);
			button.GetComponent<HorizontalSelectorPointerListener> ().initSelector (l, screenPos.y);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
