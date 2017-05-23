using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*public enum RowOrColumn
{
	ROW,
	COLUMN
}*/

//Probably only used for the booster class
public class HorizontalSelectorPointerListener : SelectorPointerListener
{
	//Below not necessary if correct callbacks are given in inspector
	/*private struct AreaOfSelection
	{
		public RowOrColumn area { get; private set; }
		public GridPosition mimickedGun { get; private set; } //a representative gun used to update scale and position of self

		public AreaOfSelection(RowOrColumn a, GridPosition n) {
			area = a;
			mimickedGun = n;
		}
	}

	private AreaOfSelection id;*/

	//[HideInInspector]

	public override void OnPointerUp(PointerEventData eventData)
	{
		//NEED TO CHANGE BEHAVIOR. selected VARIABLE SHOULD BE IN EACH GUN. CHECK HERE IF ALL GUNS ARE SELECTED BEFORE DESLECTING ALL. ETC.
		if (selected == false) {
			ship.GetComponent<WeaponManager>().SelectColumn(mimickedGun.GetComponent<Gun> ().gridPosition);
			//onSelected.Invoke ();
			selected = true;
		} else {
			ship.GetComponent<WeaponManager>().DeselectColumn(mimickedGun.GetComponent<Gun> ().gridPosition);
			//onUnSelected.Invoke ();
			selected = false;
		}
	}

	protected override void LateUpdate()
	{
		if (isSetUp) {
			Vector3 screenPos = camera.WorldToScreenPoint (mimickedGun.transform.position);
			transform.position = new Vector3 (screenPos.x, transform.position.y, transform.position.z);
		}
		
		//follow the scale of mimickedGun
		//callback function for selection
	}
}
