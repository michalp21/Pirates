using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VerticalSelectorPointerListener : SelectorPointerListener
{
	public override void OnPointerUp(PointerEventData eventData)
	{
		//NEED TO CHANGE BEHAVIOR. selected VARIABLE SHOULD BE IN EACH GUN. CHECK HERE IF ALL GUNS ARE SELECTED BEFORE DESLECTING ALL. ETC.
		if (selected == false) {
			ship.GetComponent<WeaponManager>().SelectRow(index);
			//onSelected.Invoke ();
			selected = true;
		} else {
			ship.GetComponent<WeaponManager>().DeselectRow(index);
			//onUnSelected.Invoke ();
			selected = false;
		}
	}

	protected override void LateUpdate()
	{
		float? rowPosOnScreen = ship.GetYPositionOfRowOnScreen (index);
		float? rowHeightOnScreen = ship.GetHeightOfRowOnScreen (index);
		if (rowPosOnScreen == null || rowHeightOnScreen == null) {
			gameObject.SetActive (false);
		} else {
			if (isSetUp) {
				transform.position = new Vector3 (transform.position.x, (float) rowPosOnScreen, transform.position.z);
				transform.localScale = new Vector3(1,((float) rowHeightOnScreen / initialSize),1);
			}
		}
		
		//follow the scale of mimickedGun
		//callback function for selection
	}
}
