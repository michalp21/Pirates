using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : Damager {

	protected int hitCount;
	protected bool usePool;
	public int maxReflections;
	private readonly float INFINITY = 5000;
	private LineRenderer lr;
	private int mask;

	//list for keeping track of the targets that the laser has hit
	//used for keeping track of which gameobjects to start or stop coroutines
	private List<GameObject> alreadyHit;

	public Transform limit;

	public override void SetUp(bool usePooling)
	{
		Debug.Log ("SETTING UP");
		hitCount = 0;

		//collidersToIgnore.Add (myInfo.owner.GetComponent<Collider>());
		//backCollidersToIgnore.Add (myInfo.owner.GetComponent<Collider>());
		//Invoke("Recycle", projectileLifeTime); // set a life time for this projectile
		usePool = usePooling;
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

	void Start () {
		limit = GetComponentsInChildren<Transform> ()[1];
		lr = GetComponent<LineRenderer> ();
		if (gameObject.layer == S_BULLET)
			mask = 1 << O_SHOOTER;
		else if (gameObject.layer == O_BULLET)
			mask = 1 << S_SHOOTER;
	}

	void Update () {
		lr.SetPosition (0, transform.position);
		lr.SetPosition (1, limit.position);
		RaycastHit[] hits = Physics.RaycastAll (transform.position, transform.right, INFINITY, mask);

		//create a temporary duplicate list of the objects this laser has hit
		List<GameObject> temp = new List<GameObject>(alreadyHit);

		if (hits.Length > 0) {
			foreach (RaycastHit hit in hits) {
				if (hit.collider) {
					GameObject objectHit = hit.collider.gameObject;
					if (alreadyHit.Contains (objectHit)) {
						temp.Remove (objectHit);
					} else {
						objectHit.GetComponent<Health> ().startDrain (damage);
						alreadyHit.Add (objectHit);
					}
				}
			}
		}

		foreach (GameObject e in temp) {
			e.GetComponent<Health> ().stopDrain ();
			alreadyHit.Remove (e);
		}
	}
}
