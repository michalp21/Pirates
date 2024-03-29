﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//Probably only used for the booster class
public class BoostPointerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	//[HideInInspector]
	private bool pressed = false;

	public UnityEvent onPressed;
	public UnityEvent onUnPressed;

	public void Start () {
		
	}


	public void OnPointerDown(PointerEventData eventData)
	{
		onPressed.Invoke();
		pressed = true;
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		onUnPressed.Invoke();
		pressed = false;
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		onUnPressed.Invoke();
		pressed = false;
	}
	
	void Update()
	{
		/*if (pressed)
		{
			onPressed.Invoke();
		}*/
	}
}
