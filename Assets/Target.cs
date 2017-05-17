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
	private Coroutine currentCoroutine;
	
	private Vector3 targetPoint;
	private Quaternion targetRotation;

	//Eventually get rid of this crap
	void Start () {
		weaponStats = GetComponent<WeaponStatsBase> ();
		//target = null;
		isTargeting = false;
		currentCoroutine = null;
	}

	protected void initTarget(string id)
	{
		enemyID = id;
		weaponStats = GetComponent<WeaponStatsBase> ();
		target = null;
		isTargeting = false;
		currentCoroutine = null;
	}

	protected void pointToTarget(Quaternion fromRotate, float startTime)
	{
		//Maybe have to move this back into Update()
		Vector3 lookDirection = target.transform.position - transform.position;
		float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
		Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp(fromRotate, newRotation, (Time.time - startTime) * 2f);

		/*Vector3 lookDirection = target.transform.position - transform.position;
		lookDirection.z = 0;// always pivot on z axis
		if(lookDirection.sqrMagnitude <= float.Epsilon)
			return;// can't rotate, since two points are along the same Z-axis, thus do nothing
		Quaternion newRotation = Quaternion.LookRotation(transform.forward, lookDirection.normalized);
		transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.time * .5f);*/
		//transform.LookAt(target.transform);
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
				if (objectInRange == this.gameObject) {
					i++; continue;
				}
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

	IEnumerator RotateTo() {
		float i = 0.0f;
		float rate = 1.0f / 1;

		while (i < 1.0) {
			Vector3 lookDirection = target.transform.position - transform.position;
			float angle = Mathf.Atan2 (lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
			Quaternion newRotation = Quaternion.AngleAxis (angle, Vector3.forward);
			Quaternion fromRotation = gameObject.transform.rotation;

			i += Time.deltaTime * rate;
			transform.rotation = Quaternion.Slerp (fromRotation, newRotation, i);
			yield return null;
		}
		while (true) {
			Vector3 lookDirection = target.transform.position - transform.position;
			float angle = Mathf.Atan2 (lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
			Quaternion newRotation = Quaternion.AngleAxis (angle, Vector3.forward);

			gameObject.transform.rotation = newRotation;
			yield return null;
		}
	}

	public void manualTarget()
	{
		
	}

	// Update is called once per frame
	void Update () {
		//float t = 0f;
		//Quaternion fromRotate = transform.rotation;

		if (isTargeting == false) {
			getInRangeTarget ();
			target = findClosestInRange();
			weaponsInRange.Clear();
			if (target != null) {
				isTargeting = true;
				currentCoroutine = StartCoroutine (RotateTo ());
			}
		} else {
			if (target == null || target.activeSelf == false || target.GetComponent<Health> ().isDead ||
				Vector3.Distance (gameObject.transform.position, target.transform.position) > weaponStats.targetRange) {
				isTargeting = false;
				StopCoroutine (currentCoroutine);
			} else {
				//pointToTarget (fromRotate, t);
				//StartCoroutine (RotateTo());
			}
		}
	}
}
