using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewHealth : Health {

	public GameObject hpPrefab;

	private HealthBar healthBar;

	// Use this for initialization
	void Awake(){
		GameObject hp = Instantiate (hpPrefab) as GameObject;
		healthBar = hp.GetComponent<HealthBar> ();
		Debug.Log (healthBar);

		healthBar.init (this.transform, GameObject.FindGameObjectWithTag ("hud").GetComponent<RectTransform>());

		healthBar.transform.SetParent (GameObject.FindGameObjectWithTag("hud").GetComponent<RectTransform>(), false);
//		healthBar.RepositionHealthBar ();
//		healthBar.gameObject.SetActive (true);
	}
		

	public override void Hit (int damage, Element damageType)
	{
		base.Hit (damage, damageType);
		updateHP ();

	}

	public void updateHP(){
		healthBar.updateImage (health, maxHealth);
	}

	protected override void onDrain ()
	{
		base.onDrain ();
		updateHP ();
	}

	protected override void onRegen ()
	{
		base.onRegen ();
		updateHP ();
	}

	public override void Die(){
		Destroy (healthBar);
		base.Die ();
	}
	
	
}
