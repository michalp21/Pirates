using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Probably only used for the booster class
public class PointerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public bool _pressed = false;

	public void OnPointerDown(PointerEventData eventData)
	{
		_pressed = true;
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		_pressed = false;
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		_pressed = false;
	}
	
	void Update()
	{
		if (!_pressed)
			return;


//		switch(Direction){
//		case eMovementDirection.Up:
//			break;
//		}
	}
}
