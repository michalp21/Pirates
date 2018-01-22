using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSelect : MonoBehaviour {

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 100)) {
                ShipAttack sa = hit.transform.gameObject.GetComponent<ShipAttack> ();
                Gun gun = sa.GetComponent<Gun>();
				if (sa != null) {
					if (gun.isSelf) {
						if (!sa.isSelected) {
							GetComponent<WeaponManager> ().ToggleSelectIndividual (sa.gridPosition.row, sa.gridPosition.col, true);
						} else {
							GetComponent<WeaponManager> ().ToggleSelectIndividual (sa.gridPosition.row, sa.gridPosition.col, false);
						}
					} else {
						Debug.Log ("Other weapon");
						GetComponent<WeaponManager> ().TargetSelected (gun);
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
