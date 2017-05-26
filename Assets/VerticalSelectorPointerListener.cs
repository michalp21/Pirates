using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VerticalSelectorPointerListener : SelectorPointerListener
{
	public override void OnPointerUp(PointerEventData eventData)
	{
		selected = ship.GetComponent<WeaponManager>().ToggleSelectRow(index, selected);
		Debug.Log (selected);
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
				transform.localScale = new Vector3 (initialScaleSelf.x,((float) rowHeightOnScreen / initialSize),initialScaleSelf.z);
				selected = ship.CheckIfRowIsSelected (index);
			}
		}
	}
}
