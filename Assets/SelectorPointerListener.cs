using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*public enum RowOrColumn
{
	ROW,
	COLUMN
}*/

//Probably only used for the booster class
public class SelectorPointerListener : MonoBehaviour, IPointerUpHandler
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
	public bool selected = false;
	public UnityEvent onSelected; //inspector
	public UnityEvent onUnSelected; //inspector
	public GameObject mimickedGun { get; set; } //a representative gun used to update scale and position of self

	public void OnPointerUp(PointerEventData eventData)
	{
		if (selected == false) {
			onSelected.Invoke ();
			selected = true;
		} else {
			onUnSelected.Invoke ();
			selected = false;
		}
	}

	public void helloworld () {
		Debug.Log ("Hello world");
	}
	
	void Update()
	{
		
		//follow the scale of mimickedGun
		//callback function for selection
	}
}
