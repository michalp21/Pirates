using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (WeaponStatsBase))]
public class CrewAttack : MonoBehaviour {
	public bool isSelf; //make into property when don't need inspector anymore? //as opposed to the opponent //isSelf will also be a variable in weaponstatsmanager
	public Weapon[] weapons;

	//Below variables should not be public
	protected float fireRate;              		// time between shots
	protected float nextFireTime = 0.0f;        // able to fire again on this frame

	void Start () {
		//0: default
		//1-4: specials
		weapons = new Weapon[4];
	}

	protected void initCrewAttack(bool i) 
	{
		isSelf = i;
	}

	public void AttackDefault(){
		//weapons [0].Attack ();
	}

	void Update () {
		
	}
}
