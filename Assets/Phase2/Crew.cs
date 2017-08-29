using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : MonoBehaviour {
	public string name;
	CrewWeapon myCrewWeapon; //maybe don't need?

	// Use this for initialization
	void Start () {
		myCrewWeapon = GetComponent<CrewWeapon> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
