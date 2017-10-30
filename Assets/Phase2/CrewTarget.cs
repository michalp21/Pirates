using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class CrewTarget : MonoBehaviour {

	Ray ray;
	RaycastHit hitInfo;
	public float personalSpace;
	MovementAI myMovementAI;
	CrewAttack myCrewAttack;
	GameObject manualTarget;
	GameObject goal;
	Collider[] hitColliders;
	List<GameObject> targetsInRange = new List<GameObject>(); //DS
	const int GET_TARGET_RANGE = 10;

	//Assumes LAYERS and TAGS are set upon instantiation of crew prefab
	//eg Destination = reptile1 = reptile2 = <layer11> ; Destination = <tag"Goal"> ; reptile1 = reptile2 = <tag"Crew">
	void GetInRangeTarget() {
		hitColliders = Physics.OverlapSphere (gameObject.transform.position, GET_TARGET_RANGE);
		//Filter by tags: get only enemy gameobjects
		if (hitColliders != null) {
			targetsInRange = hitColliders
			.Where (c =>
				(c.gameObject.tag.Contains ("Crew") || c.gameObject.tag.Contains ("ManualTarget")) &&
				c.gameObject.layer != gameObject.layer)
			.Select (c => c.gameObject).ToList ();
		}
		//goal should NOT be null once both ships are added
		if (goal != null)
			targetsInRange.Add (goal);
	}

	protected void FindClosestInRange()
	{
		//loop through all gameobjects in DS and find closest one
		float distance;
		float minDistance = float.MaxValue;
		GameObject closestObject = null;
		for (int i = 0; i < targetsInRange.Count; i++) {
			distance = Vector3.Distance (gameObject.transform.position, targetsInRange[i].transform.position);
			if(distance < minDistance) {
				minDistance = distance;
				closestObject = targetsInRange[i];
			}
		}
		if (closestObject != null) {
			Debug.Log(gameObject.name + " : " + closestObject);
			myMovementAI.target = closestObject.transform;
		}
		else
			myMovementAI.target = null;
	}

	bool checkCollision() {
		Debug.DrawRay (transform.position + new Vector3(0f,.5f,0f), transform.forward * personalSpace, Color.yellow);
		if (Physics.Raycast (transform.position + new Vector3(0f,.5f,0f), transform.forward, out hitInfo, personalSpace)) {
			//Stop movement, start attacking
			if (hitInfo.collider.tag == "Crew" && hitInfo.collider.gameObject.layer != gameObject.layer) {
				myMovementAI.speed = 0;
				myCrewAttack.Attack ();
				return true;
			}
		}
		return false;
	}

	void SetTarget() {
		manualTarget = null;
		GetInRangeTarget ();
		FindClosestInRange ();
		targetsInRange.Clear ();
	}

	void Start () {
		myMovementAI = GetComponent<MovementAI> ();
		myCrewAttack = GetComponent<CrewAttack> ();
		goal = GameObject.FindGameObjectsWithTag ("Goal").ToList().Find (g => g.layer != this.gameObject.layer);
		SetTarget ();
		//Only update path when another character is initialized
		CrewManager.onInstantiated += SetTarget;
	}

	void OnDisable () {
		CrewManager.onInstantiated -= SetTarget;
	}

	void Update () {
		checkCollision ();
	}
}
