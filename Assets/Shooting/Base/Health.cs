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
	protected float maxHealth;
	public float health;

	public Slider healthBarSlider;

	//deal with this later
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

    public virtual void Hit(ProjectileInfo info)
    {
        if (isInvincible)
        {
            return;
        }

        if (!isDead)
        {
            int damage = info.damage.amount;

            switch (info.damage.type)
            {
                case DamageType.NORMAL:
                    damage = Mathf.RoundToInt(damage * myResistances.normal);
                    break;
                case DamageType.FIRE:
                    damage *= Mathf.RoundToInt(damage * myResistances.fire);
                    break;
                case DamageType.ICE:
                    damage *= Mathf.RoundToInt(damage * myResistances.ice);
                    break;
                case DamageType.ACID:
                    damage *= Mathf.RoundToInt(damage * myResistances.acid);
                    break;
                case DamageType.ELECTRIC:
                    damage *= Mathf.RoundToInt(damage * myResistances.electric);
                    break;
                case DamageType.POISON:
                    damage *= Mathf.RoundToInt(damage * myResistances.poison);
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

    public virtual void Regen()
    {
        if ((health < maxHealth))
        {
            health += regenAmount;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
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