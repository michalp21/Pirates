using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HorizontalSelectorPointerListener : SelectorPointerListener
{
	public override void OnPointerUp(PointerEventData eventData)
	{
		selected = ship.GetComponent<WeaponManager>().ToggleSelectColumn(index, selected);
		//Debug.Log (selected);
	}

	protected override void LateUpdate()
	{
		//check using state of Column object maybe?
		float? colPosOnScreen = ship.GetXPositionOfColOnScreen (index);
		float? colWidthOnScreen = ship.GetWidthOfColOnScreen (index);

		if (colPosOnScreen == null || colWidthOnScreen == null) {
			gameObject.SetActive (false);
		} else {
			if (isSetUp) {
				transform.position = new Vector3 ((float) colPosOnScreen, transform.position.y, transform.position.z);
				transform.localScale = new Vector3(((float) colWidthOnScreen / initialSize),initialScaleSelf.y,initialScaleSelf.z);
				selected = ship.CheckIfColumnIsSelected (index);
			}
		}
	}
}
