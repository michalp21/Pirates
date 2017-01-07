using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Target : MonoBehaviour {

	private List<GameObject> weaponsInRange = new List<GameObject>(); //DS
	private WeaponStatsBase weaponStats;
	//maybe add a targeting variable to weaponStats (enemy or player)
	
	public string enemyID; //not used or implemented yet

	private bool isTargeting;
	private bool isManual; //if false: auto targeting
	private GameObject target;
	private Collider[] hitColliders;
	
	private Vector3 targetPoint;
	private Quaternion targetRotation;
	
	void Start () {
		
	}

	protected void initTarget(string id)
	{
		enemyID = id;
		weaponStats = GetComponent<WeaponStatsBase> ();
		target = null;
	}

	protected void pointToTarget()
	{
		//Maybe have to move this back into Update()
		var newRotation = Quaternion.LookRotation(target.transform.position - transform.position);
		newRotation.x = 0.0f;
		newRotation.y = 0.0f;
		transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * .5f);
	}
	
	protected GameObject findClosestInRange()
	{
		//loop through all gameobjects in DS and find closest one
		float distance;
		float minDistance = float.MaxValue;
		GameObject closestObject = null;
		for (int i = 0; i < weaponsInRange.Count; i++) {
			distance = Vector3.Distance (gameObject.transform.position, weaponsInRange[i].transform.position);
			if(distance < minDistance) {
				minDistance = distance;
				closestObject = weaponsInRange[i];
			}
		}

		return closestObject;
	}

	//call once before attacking
	protected void getInRangeTarget()
	{
		hitColliders = Physics.OverlapSphere(gameObject.transform.position,
		                                     weaponStats.targetRange);
		if (hitColliders != null) {
			int i = 0;
			while (i < hitColliders.Length) {
				GameObject objectInRange = hitColliders[i].gameObject;
				Gun gun = objectInRange.GetComponent<Gun>();
				if (gun != null &&								//is a gun
				    objectInRange.tag.Contains (enemyID) &&		//is an enemy
				    !weaponsInRange.Contains (objectInRange)) {	//is not already in DS
					weaponsInRange.Add (objectInRange);			//add to DS
				}
				i++;
			}
		}
	}

	void manualTarget()
	{

	}

	// Update is called once per frame
	void Update () {
		if (!isTargeting) {
			getInRangeTarget ();
			target = findClosestInRange();
			weaponsInRange.Clear();
			isTargeting = true;
		} else {
			if (target == null || target.activeSelf == false || target.GetComponent<Health>().isDead)
				isTargeting = false;
			else
				pointToTarget ();
		}
	}
}
