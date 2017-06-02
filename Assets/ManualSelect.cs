using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSelect : MonoBehaviour {

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 100)) {
				Gun weapon = hit.transform.gameObject.GetComponent<Gun> ();
				if (weapon != null) {
					if (weapon.isSelf) {
						if (!weapon.isSelected) {
							GetComponent<WeaponManager> ().ToggleSelectIndividual (weapon.gridPosition.row, weapon.gridPosition.col, true);
						} else {
							GetComponent<WeaponManager> ().ToggleSelectIndividual (weapon.gridPosition.row, weapon.gridPosition.col, false);
						}
					} else {
						Debug.Log ("Other weapon");
						GetComponent<WeaponManager> ().TargetSelected (weapon);
					}
				}
			}
		}
	}

	/*void OnMouseDown()
	{
		Debug.Log ("click");
		if (GetComponent<Gun> ().isSelected == true) {
			GetComponent<Gun> ().isSelected = false;
			GetComponent<SpriteRenderer> ().enabled = false;
		} else {
			GetComponent<Gun> ().isSelected = true;
			GetComponent<SpriteRenderer> ().enabled = true;
		}
	}*/
}
