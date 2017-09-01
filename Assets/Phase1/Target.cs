using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Target : MonoBehaviour {

	List<GameObject> weaponsInRange = new List<GameObject>(); //DS
	WeaponStatsBase weaponStats;
	//maybe add a targeting variable to weaponStats (enemy or player)
	
	//public string enemyID; //not used or implemented yet

	bool isLockedOn;
	GameObject target;
	Collider[] hitColliders;
	Coroutine currentCoroutine;
	
	Vector3 targetPoint;
	Quaternion targetRotation;

	public bool isManual { get; set; } //if false: auto targeting
	public bool canFire { get; set; }

	void Start () {
		weaponStats = GetComponentInParent<WeaponStatsBase> ();
		//target = null;
		isLockedOn = false;
		isManual = false;
		currentCoroutine = null;
	}

	/*protected void initTarget(string id)
	{
		//enemyID = id;
		weaponStats = GetComponentInParent<WeaponStatsBase> ();
		target = null;
		isTargeting = false;
		currentCoroutine = null;
	}*/

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

	//call once before attacking
	protected void getInRangeTarget()
	{
		hitColliders = Physics.OverlapSphere (gameObject.transform.position, weaponStats.targetRange);
		if (hitColliders != null) {
			int i = 0;
			while (i < hitColliders.Length) {
				GameObject objectInRange = hitColliders[i].gameObject;
				if (objectInRange == this.gameObject) {
					i++; continue;
				}
				Gun gun = objectInRange.GetComponent<Gun>();
				if (gun != null &&										//is a gun
					objectInRange.gameObject.tag.Contains("Weapon") &&	//is the parent object
					objectInRange.layer != gameObject.layer &&  		//is an enemy
					!weaponsInRange.Contains (objectInRange)) {			//is not already in DS
					weaponsInRange.Add (objectInRange);					//add to DS
				}
				i++;
			}
		}
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

	IEnumerator RotateTo() {
		float i = 0.0f;
		float rate = 1.0f / 1;

		canFire = false;
		while (i < 1.0) {
			Vector3 lookDirection = target.transform.position - transform.position;
			float angle = Mathf.Atan2 (lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
			Quaternion newRotation = Quaternion.AngleAxis (angle, Vector3.forward);
			Quaternion fromRotation = gameObject.transform.rotation;

			i += Time.deltaTime * rate;
			transform.rotation = Quaternion.Slerp (fromRotation, newRotation, i);
			yield return null;
		}
		canFire = true;
		while (target != null) {
			Vector3 lookDirection = target.transform.position - transform.position;
			float angle = Mathf.Atan2 (lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
			Quaternion newRotation = Quaternion.AngleAxis (angle, Vector3.forward);

			gameObject.transform.rotation = newRotation;
			yield return null;
		}
	}

	public void manualTarget(Gun newTarget)
	{
		if (newTarget != null) {
			target = newTarget.gameObject;
			isManual = true;
			isLockedOn = true;
			StopCoroutine (currentCoroutine);
			currentCoroutine = StartCoroutine (RotateTo ());
		}
	}

	// Update is called once per frame
	void Update () {
		//float t = 0f;
		//Quaternion fromRotate = transform.rotation;

		if (!isManual) {
			if (!isLockedOn) {
				getInRangeTarget ();
				target = findClosestInRange ();
				weaponsInRange.Clear ();
				if (target != null) {
					isLockedOn = true;
					currentCoroutine = StartCoroutine (RotateTo ());
				}
			} else {
				if (target == null || target.activeSelf == false || target.GetComponent<Health> ().isDead ||
				    Vector3.Distance (gameObject.transform.position, target.transform.position) > weaponStats.targetRange) {
					isLockedOn = false;
					StopCoroutine (currentCoroutine);
				}
			}
		} else {
			if (target == null || target.activeSelf == false || target.GetComponent<Health> ().isDead ||
			    Vector3.Distance (gameObject.transform.position, target.transform.position) > weaponStats.targetRange) {
				isLockedOn = false;
				StopCoroutine (currentCoroutine);
			}
		}
	}
}
