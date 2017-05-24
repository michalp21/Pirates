using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class SelectorPointerListener : MonoBehaviour, IPointerUpHandler {
	public bool selected;
	public UnityEvent onSelected;
	public UnityEvent onUnSelected;
	//public GameObject mimickedGun { get; set; } //a representative gun used to update scale and position of self
	public bool isSetUp { get; set; }
	public Camera camera;
	public WeaponManager ship;
	public float initialRatio; //obj height divided by screen height
	public int index; //row or column

	public void initSelector (int i, float yObjPos)
	{
		selected = false;
		camera = Camera.main;
		//mimickedGun = someGun;
		isSetUp = true;
		ship = GameObject.Find ("Ship_self").GetComponent<WeaponManager> ();
		initialRatio = yObjPos / (float)Screen.height;
		index = i;
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		
	}
	
	// Update is called once per frame
	protected virtual void LateUpdate () {
		
	}
}
