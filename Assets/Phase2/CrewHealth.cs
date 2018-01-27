using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewHealth : Health {

    public GameObject hpPrefab;
    protected HealthBar healthBar;

	// Use this for initialization
	protected void Awake(){
        GameObject hp = Instantiate(hpPrefab) as GameObject;
        healthBar = hp.GetComponent<HealthBar>();
        health = maxHealth = 100;
        RectTransform hudTransform = GameObject.FindGameObjectWithTag("hud").GetComponent<RectTransform>();
        healthBar.init(this.transform, hudTransform);
        //set parent transform but keep local orientation
        healthBar.transform.SetParent(hudTransform, false);
	}

    public override void ResetMe(){
        health = maxHealth;
    }

	public override void Hit (int damage, Element damageType)
	{
		base.Hit (damage, damageType);
        updateHP();
	}

	protected override void onDrain ()
	{
		base.onDrain ();
        updateHP();
	}

	protected override void onRegen ()
	{
		base.onRegen ();
        updateHP();
	}

	public override void Die(){
		base.Die ();
        Destroy(healthBar);
	}

    public virtual void updateHP()
    {
        healthBar.updateImage(health, maxHealth);
    }
	
}
