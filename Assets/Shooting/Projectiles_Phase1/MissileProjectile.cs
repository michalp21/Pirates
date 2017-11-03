using UnityEngine;
using System.Collections;

public class MissileProjectile : Projectile
{
	//consider moving to weaponstatsbase
	public float blastRadius;
	public float numberOfBlasts;
	public float blastRadiusMultiple;

	private int enemyLayer;

	private SpriteRenderer explosionGraphic;

	public override void SetUp (bool usePooling) {
		base.SetUp (usePooling);
		explosionGraphic = GetComponentInChildren<SpriteRenderer> ();

		if (gameObject.layer == S_BULLET)
			enemyLayer = O_SHOOTER;
		else if (gameObject.layer == O_BULLET)
			enemyLayer = S_SHOOTER;
	}

	protected override void OnTriggerEnter(Collider other)
	{
		// if we hit water... kill the bullet IF it won't survive underwater (not fully implemented yet)
		/*if (other.gameObject.CompareTag("Water") &&
		   (myInfo.psurvive == ProjectileSurvive.AIR))
		{
			CancelInvoke("Recycle");
			Recycle();
		}*/

		//do one for air too


		if (other.gameObject.tag.Contains("Weapon"))
		{
			//coll.GetComponent<healthScript>().health -= 1;
			Collider[] hitColliders = Physics.OverlapSphere (gameObject.transform.position, blastRadius);
			if (hitColliders != null) {
				foreach(Collider hitCollider in hitColliders) {
					if (hitCollider != null && hitCollider.gameObject.tag.Contains("Weapon") && hitCollider.gameObject.layer == enemyLayer) {
						Health hitObject = hitCollider.gameObject.GetComponent<Health>();
						hitObject.Hit(damage, damageType); // send bullet info to hit object's health component
					}
				}
			}

			hitCount++; // add a hit

			if (hitCount >= maxPenetration)
			{
				CancelInvoke("Recycle");
				Explode(); // if hit count exceeds max hits.... kill the bullet
			}
		}
	}

	protected void Explode ()
	{
		myRigid.velocity = new Vector3 (0, 0, 0);
		GetComponent<MeshRenderer> ().enabled = false;
		explosionGraphic.enabled = true;
		Invoke ("Recycle", .1f);
	}
}
