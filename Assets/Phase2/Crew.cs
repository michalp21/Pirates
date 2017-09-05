using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : MonoBehaviour {
	public string name;
	CrewAttack myCrewWeapon; //maybe don't need?

	// Use this for initialization
	void Start () {
		myCrewWeapon = GetComponent<CrewAttack> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
