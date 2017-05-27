using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  Base Class for projectiles, contains common elements to all type of projectiles for other projectile  classes to be derived from.
/// </summary>

public abstract class Projectile : MonoBehaviour {
	public ProjectileSurvive psurvive;
	//public GameObject owner;
	//public string enemyID;
	public string name;
	public int damage;
	public Element damageType;
	public int maxPenetration;
	public float spread;
	public float speed;
	public float projectileLifeTime;

    protected Vector3 velocity;
	protected int hitCount;
    //protected List<Collider> collidersToIgnore = new List<Collider>();
    //protected List<Collider> backCollidersToIgnore = new List<Collider>();
	protected Rigidbody myRigid;
	protected bool usePool;

    // This is bullet initialization, It gets called by the weapon that fired this projectile
	public virtual void SetUp(bool usePooling)
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

	//UPDATE THIS TO WORK FOR TRIGGERS, not RAYCASTS (changed to OnTriggerEnter)
	//    void FixedUpdate()
	//    {
	//        RaycastHit hit;  // forward hit
	//        RaycastHit hit2; // rear hit       
	//        
	//        if ()
	//        {
	//            // probably shouldn't do this but best way i can think of to avoid
	//            // multiple hits from same bullet
	//            myRigid.MovePosition(hit.point); // move the bullet to the impact point
	//            transform.position = hit.point;
	//            
	//            if (hit.transform.CompareTag("Water"))
	//            {// if we hit water... kill the bullet IF it won't survive underwater (not fully implemented yet)
	//                CancelInvoke("Recycle");
	//                Recycle();
	//            }
	//
	//            Health hitObject = hit.transform.GetComponent<Health>();
	//
	//            if (hitObject)
	//            {                
	//                hitObject.Hit(myInfo); // send bullet info to hit object's health component
	//            }
	//			else
	//            {
	//                //MakeAHole(hit); // make a hole anywhere except the players
	//            }
	//	
	//            hitCount++; // add a hit
	//
	//            if (hitCount > myInfo.maxPenetration)
	//            {
	//                CancelInvoke("Recycle");
	//                Recycle(); // if hit count exceeds max hits.... kill the bullet
	//            }
	//        }
	//    }  
   
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
