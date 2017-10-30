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

	public Transform limit;

	private readonly int sWpnLayer = 11;
	private readonly int oWpnLayer = 12;

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
		if (gameObject.layer == 9)
			mask = 1 << oWpnLayer;
		else if (gameObject.layer == 10)
			mask = 1 << sWpnLayer;
	}

	void Update () {
		lr.SetPosition (0, transform.position);
		lr.SetPosition (1, limit.position);
		RaycastHit[] hits = Physics.RaycastAll (transform.position, transform.right, INFINITY, mask);
		if (hits.Length > 0) {
			foreach (RaycastHit hit in hits) {
				if (hit.collider) {
					StartCoroutine (hit.collider.GetComponent<Health> ().Drain (damage));
				}
			}
		}
	}
}
