using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*[System.Serializable]
public class ProjectileInfo
{
	public GameObject owner;
	public string name;
	public Damage damage = new Damage();
	public float force;
	public int maxPenetration;
	public float maxSpread;
	public float spread;
	public float speed;
	public bool usePool;
	public float projectileLifeTime;
}*/


/// <summary>
///  Base Class for projectiles, contains common elements to all type of projectiles for other projectile  classes to be derived from.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : Damager {

	public int maxPenetration;
	public float spread;
	public float speed;
	public float projectileLifeTime;

	protected int hitCount;
    protected Vector3 velocity;
    //protected List<Collider> collidersToIgnore = new List<Collider>();
    //protected List<Collider> backCollidersToIgnore = new List<Collider>();
	protected Rigidbody myRigid;
	protected bool usePool;

    // This is bullet initialization, It gets called by the weapon that fired this projectile
	//GET RID OF THIS
	public override void SetUp(bool usePooling)
    {
        hitCount = 0;
		velocity = speed * transform.right + transform.TransformDirection(0, Random.Range(-spread/2, spread/2), 0);
		//collidersToIgnore.Add (myInfo.owner.GetComponent<Collider>());
		//backCollidersToIgnore.Add (myInfo.owner.GetComponent<Collider>());
        Invoke("Recycle", projectileLifeTime); // set a life time for this projectile
		myRigid = GetComponent<Rigidbody>();       
		myRigid.velocity = velocity;
		usePool = usePooling;
    }

	protected virtual void OnTriggerEnter(Collider other)
	{

	}
   
    protected virtual void Recycle()
    {
		//Clear the colliders this bullet ignores
		//collidersToIgnore.Clear ();
		//backCollidersToIgnore.Clear ();

        // pool or destroy the bullet when it is no longer used.
        if (usePool)
        {
            ObjectPool.pool.PoolObject(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
