using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
	public WeaponStatsBase weaponStats;
	public WeaponManager weaponManager;

    // my resistance to different damage types 
    // you basically want resistances between 0 and 2, where 0 is totally immune to damage type,
    // 1 takes straight damage as in no resistance or weakness and 2 is take double damage
	public Resistance myResistances;

	public bool useObjectPooling;
    public int regenAmount = 1;
    public bool isDead = false;
	protected bool isInvincible = false;
	public float maxHealth { get; set; }
	public float health;

	public Slider healthBarSlider;

	protected float nextDrainTime = 0.0f;
	protected float nextRegenTime = 0.0f;
	protected float drainRate;	//time between each drain
	protected float regenRate;	//time between each regen
	protected int DRAIN_PER_FRAME = 1;
	protected int REGEN_PER_FRAME = 1;

	void Start ()
	{
		weaponStats = GetComponentInChildren<WeaponStatsBase> (); //maybe change to 2nd parameter (See above)
		useObjectPooling = weaponStats.usePooling;
		myResistances = weaponStats.myResistances;
		maxHealth = weaponStats.health;
		healthBarSlider = GetComponentInChildren<Slider> ();
		healthBarSlider.value = healthBarSlider.maxValue = health = maxHealth;

		weaponManager = GetComponentInParent<WeaponManager> ();
	}

	/*public virtual void initHealth()
	{
		weaponStats = GetComponentInChildren<WeaponStatsBase>(); //maybe change to 2nd parameter (See above)
		useObjectPooling = weaponStats.usePooling;
		myResistances = weaponStats.myResistances;
		maxHealth = weaponStats.health;
		healthBarSlider = GetComponentInChildren<Slider> ();
		healthBarSlider.value = healthBarSlider.maxValue = health = maxHealth;
	}*/

    public virtual void ResetMe()
    {
        isDead = false; // make sure its not dead to start 
        health = maxHealth;
		healthBarSlider.value = health;
    }

	public virtual void Hit(int damage, Element damageType)
    {
        if (isInvincible)
        {
            return;
        }

        if (!isDead)
        {
			switch (damageType)
            {
                case Element.NORMAL:
                    damage = Mathf.RoundToInt(damage * myResistances.normal);
                    break;
                case Element.FIRE:
                    damage = Mathf.RoundToInt(damage * myResistances.fire);
                    break;
				case Element.ICE:
                    damage = Mathf.RoundToInt(damage * myResistances.ice);
                    break;
				case Element.ACID:
                    damage = Mathf.RoundToInt(damage * myResistances.acid);
                    break;
				case Element.ELECTRIC:
                    damage = Mathf.RoundToInt(damage * myResistances.electric);
                    break;
                case Element.POISON:
                    damage = Mathf.RoundToInt(damage * myResistances.poison);
                    break;
                default:
                    // take straight damage...
                    break;
            }
            health -= damage;

			weaponManager.TakeDamage (damage);
			healthBarSlider.value -= damage;

			if (health <= 0)
            {
                health = 0;
                isDead = true;
                Die();
				weaponManager.RemoveWeapon (gameObject.GetComponent<Gun> ());
            }
        }
    }
	//param drainRate: health drained per second
	public IEnumerator Drain(int dps)
	{
		while (health > 0) {
			if (nextDrainTime < Time.time) {
				
				if (!isInvincible && !isDead) {
					health -= DRAIN_PER_FRAME;
					weaponManager.TakeDamage (DRAIN_PER_FRAME);
					healthBarSlider.value -= DRAIN_PER_FRAME;

					if (health <= 0) {
						health = 0;
						isDead = true;
						Die ();
						weaponManager.RemoveWeapon (gameObject.GetComponent<Gun> ());
					}
				}
				nextDrainTime = Time.time + DRAIN_PER_FRAME / (float)dps;
			}
			yield return null;
		}
	}

	//param drainRate: health regenerated per second
	public IEnumerator Regen(int rps)
    {
		while (true) {
			if (nextDrainTime < Time.time) {
				
				if ((health < maxHealth)) {
					health += REGEN_PER_FRAME;
					if (health > maxHealth) {
						health = maxHealth;
					}
				}
				nextRegenTime = Time.time + REGEN_PER_FRAME / (float)rps;
			}
			yield return null;
		}
    }

    public virtual void Die()
    {
        // send any messages to the player script here to tell it it's dead and to stop taking input
        // or any other script you might need to let know that it died
        if (useObjectPooling)
        {
            ObjectPool.pool.PoolObject(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
		
		

[System.Serializable]
public class Resistance
{
    public float normal;
    public float fire;
    public float ice;
    public float acid;
    public float electric;
    public float poison;
}