using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class SelectorPointerListener : MonoBehaviour, IPointerUpHandler {
	public bool selected;
	public UnityEvent onSelected;
	public UnityEvent onUnSelected;
	public GameObject mimickedGun { get; set; } //a representative gun used to update scale and position of self
	public bool isSetUp { get; set; }
	public Camera camera;
	public GameObject ship;

	public void initSelector (GameObject someGun)
	{
		selected = false;
		camera = Camera.main;
		mimickedGun = someGun;
		isSetUp = true;
		ship = GameObject.Find ("Ship_self");
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		
	}
	
	// Update is called once per frame
	protected virtual void LateUpdate () {
		
	}
}
