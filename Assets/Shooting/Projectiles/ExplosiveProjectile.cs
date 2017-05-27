using UnityEngine;
using System.Collections;

public abstract class ExplosiveProjectile : Projectile
{
	public float blastRadius; //blast radius
	public float numberOfBlasts; //number of blasts
	public float blastRadiusMultiple; //radius in which blasts appear

	protected virtual void Explode(Collider other)
	{

	}  
}
