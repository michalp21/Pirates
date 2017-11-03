using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damager : MonoBehaviour {
	public ProjectileSurvive psurvive;
	public int damage;
	public Element damageType;

	//constants for tagging self or enemy shooter/bullets
	public const int S_BULLET = 9, O_BULLET = 10, S_SHOOTER = 11, O_SHOOTER = 12;

	public virtual void SetUp (bool usePooling)
	{
		
	}
}
