using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class SelectorPointerListener : MonoBehaviour, IPointerUpHandler {
	public bool selected;
	public UnityEvent onSelected;
	public UnityEvent onUnSelected;
	public bool isSetUp { get; set; }
	public Camera camera;
	public WeaponManager ship;
	public Vector3 initialScaleSelf; //initial scale of self
	public float initialSize; //width or height of column or row, respectively
	public int index; //row or column

	public void initSelector (int i, float range)
	{
		selected = false;
		camera = Camera.main;
		//mimickedGun = someGun;
		isSetUp = true;
		ship = GameObject.Find ("Ship_self").GetComponent<WeaponManager> ();
		initialScaleSelf = transform.localScale;
		initialSize = range;
		index = i;
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		
	}

	protected virtual void LateUpdate () {
		
	}
}
