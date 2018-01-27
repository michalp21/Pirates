using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHealth : Health {

	public WeaponStatsBase weaponStats;
	public WeaponManager weaponManager;

    //protected RectTransform canvasRT;

    public Slider healthBarSlider;

    //protected override void Awake() {
    //    base.Awake();
    //    canvasRT = transform.Find("Canvas").GetComponent<RectTransform>();
    //    healthBar.init(this.transform, canvasRT);
    //    //set parent transform but keep local orientation
    //    healthBar.transform.SetParent(canvasRT, false);
    //}

	// Use this for initialization
	protected override void Start () {
        base.Start();
		weaponStats = GetComponent<WeaponStatsBase> (); //maybe change to 2nd parameter (See above)
		useObjectPooling = weaponStats.usePooling;
		myResistances = weaponStats.myResistances;
		//health = maxHealth = weaponStats.health;

		weaponManager = GetComponentInParent<WeaponManager> ();

        healthBarSlider = GetComponentInChildren<Slider>();
        healthBarSlider.value = healthBarSlider.maxValue = health = maxHealth = weaponStats.health;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Hit (int damage, Element damageType)
	{
		base.Hit (damage, damageType);
        healthBarSlider.value -= damage;
		weaponManager.TakeDamage (damage);

	}

	protected override void onDrain ()
	{
		base.onDrain ();
        healthBarSlider.value -= DRAIN_PER_FRAME;
		weaponManager.TakeDamage (DRAIN_PER_FRAME);
	}

    protected override void onRegen()
    {
        base.onRegen();
        healthBarSlider.value += REGEN_PER_FRAME;
        weaponManager.TakeDamage(-REGEN_PER_FRAME);
    }

    public override void Die()
    {
        healthBarSlider.value -= health;
        weaponManager.TakeDamage(health);
        Debug.Log("YOYOYO");
        base.Die();
    }
}
