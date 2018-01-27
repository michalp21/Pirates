using UnityEngine;
using System.Collections;
//using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // resistance to different damage types 
    // you basically want resistances between 0 and 2, where 0 is totally immune to damage type,
    // 1 takes straight damage as in no resistance or weakness and 2 is take double damage
	public Resistance myResistances;

	public bool useObjectPooling;
    public int regenAmount = 1;
    public bool isDead = false;
	protected bool isInvincible = false;
	public int maxHealth { get; set; }
	public int health;

	public int draining { get; set; }
    public int regening { get; set; }
	

	protected float nextDrainTime = 0.0f;
	protected float nextRegenTime = 0.0f;
	protected float drainRate;	//time between each drain
	protected float regenRate;	//time between each regen
	protected int DRAIN_PER_FRAME = 1;
	protected int REGEN_PER_FRAME = 1;

	//coroutine for draining from laser / other damagers
	private Coroutine drainCoroutine, regenCoroutine;

	private float inv_start;
	public float inv_duration;

    protected virtual void Start ()
    {
		StopAllCoroutines ();
	}

    public virtual void ResetMe()
    {
        isDead = false; // make sure its not dead to start
    }

	public virtual void Hit(int damage, Element damageType)
    {
        Debug.Log(damage);
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
                    Debug.Log(myResistances.normal);
                    Debug.Log(damage);
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
			Debug.Log ("current hp: " + health + ", incoming damage: " + damage);
            health -= damage;
			isInvincible = true;
			inv_start = Time.time;
            if (health <= 0)
            {
                Debug.Log("DEATH!!!");
                Die();
            }

        }
    }

	protected void FixedUpdate(){
		if (isInvincible) {
			float elapsed = Time.time - inv_start;
			if (elapsed >= inv_duration) {
				isInvincible = false;
				inv_start = 0;
			}


		}
	}

	public void startDrain(int dps){
		drainCoroutine = StartCoroutine (Drain (dps));
		draining++;
	}

	public void stopDrain(){
		if (drainCoroutine != null) {
			StopCoroutine (drainCoroutine);
		}
		draining--;
		if (draining < 0) {
			draining = 0;
		}
	}

	public void startRegen(int hps){
		regenCoroutine = StartCoroutine (Regen (hps));
		regening++;
	}

	public void stopRegen(){
		if (regenCoroutine != null) {
			StopCoroutine (regenCoroutine);
		}
		regening--;

	}

	//param drainRate: health drained per second
	public IEnumerator Drain(int dps)
	{
		while (health > 0 && draining > 0) {
			if (nextDrainTime < Time.time) {
				
				if (!isInvincible && !isDead) {
					onDrain ();

					if (health <= 0) {
						Die ();
					}
				}
				nextDrainTime = Time.time + DRAIN_PER_FRAME / (float)dps;
			}
			yield return null;
		}
	}

	protected virtual void onDrain(){
		health -= DRAIN_PER_FRAME;
	}

	//param drainRate: health regenerated per second
	public IEnumerator Regen(int rps)
    {
		while (health < maxHealth && regening > 0) {
			if (nextDrainTime < Time.time) {
				
				if ((health < maxHealth)) {
					onRegen ();
//					healthBarSlider.value += REGEN_PER_FRAME;
				}

				nextRegenTime = Time.time + REGEN_PER_FRAME / (float)rps;
			}
			yield return null;
		}
    }

	protected virtual void onRegen(){
		health += REGEN_PER_FRAME;
		if (health > maxHealth) {
			health = maxHealth;
		}
	}

    public virtual void Die()
	{
        Debug.Log("IHIHIHIHIH");
		health = 0;
		isDead = true;

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