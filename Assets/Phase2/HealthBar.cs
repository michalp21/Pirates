using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	RectTransform canvas;
	Transform target;

	public void init(Transform t, RectTransform hud){
		canvas = hud;
		target = t;

		RepositionHealthBar ();
		gameObject.SetActive (true);
	}

	public void updateImage(float hp, float max){
		GetComponent<Image> ().fillAmount = hp/max;
	}

	void Update(){
		RepositionHealthBar ();
	}

	public void RepositionHealthBar()
	{
		Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.position);
		Vector2 WorldObject_ScreenPosition = new Vector2(
			((ViewportPosition.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
			((ViewportPosition.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)) + 30);
		//now you can set the position of the ui element
		GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
	}
}
