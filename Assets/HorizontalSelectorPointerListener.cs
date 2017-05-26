using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HorizontalSelectorPointerListener : SelectorPointerListener
{
	public override void OnPointerUp(PointerEventData eventData)
	{
		//NEED TO CHANGE BEHAVIOR. selected VARIABLE SHOULD BE IN EACH GUN. CHECK HERE IF ALL GUNS ARE SELECTED BEFORE DESLECTING ALL. ETC.
		if (selected == false) {
			ship.GetComponent<WeaponManager>().SelectColumn(index);
			//onSelected.Invoke ();
			selected = true;
		} else {
			ship.GetComponent<WeaponManager>().DeselectColumn(index);
			//onUnSelected.Invoke ();
			selected = false;
		}
	}

	protected override void LateUpdate()
	{
		float? colPosOnScreen = ship.GetXPositionOfColOnScreen (index);
		float? colWidthOnScreen = ship.GetWidthOfColOnScreen (index);
		if (colPosOnScreen == null || colWidthOnScreen == null) {
			gameObject.SetActive (false);
		} else {
			if (isSetUp) {
				transform.position = new Vector3 ((float) colPosOnScreen, transform.position.y, transform.position.z);
				transform.localScale = new Vector3(((float) colWidthOnScreen / initialSize),1,1);
			}
		}

		//follow the scale of mimickedGun
		//callback function for selection
	}
}
