using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewTarget : MonoBehaviour {

	public float ENGAGE_TARGET_LONGDIST = 2f;
	public float ENGAGE_TARGET_SHORTDIST = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Find closest target in the list of target colliders

		Collider[] collidersLong = Physics.OverlapSphere (transform.position, ENGAGE_TARGET_LONGDIST);
		Collider[] collidersShort = Physics.OverlapSphere (transform.position, ENGAGE_TARGET_SHORTDIST);
	}
}
