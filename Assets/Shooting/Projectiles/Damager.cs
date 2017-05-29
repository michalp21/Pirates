using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damager : MonoBehaviour {
	public ProjectileSurvive psurvive;
	public int damage;
	public Element damageType;

	public virtual void SetUp (bool usePooling)
	{
		
	}
}
