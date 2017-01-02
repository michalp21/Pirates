using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Target : MonoBehaviour {
	private List<GameObject> weaponsInRange= new List<GameObject>();
	private WeaponStatsBase weaponStats;
	private int currentLevel;

	public float targetRange;

	private isTargeting;
	private isManual;

	void Start () {
		
	}

	protected void initGun(int cl, string id)
	{
		currentLevel = cl;
		weaponStats = GetComponent<WeaponStatsBase> ();
		weaponStats.initWeaponStats (currentLevel);

		targetRange = weaponStats.targetRange;
	}

	void autoTarget()
	{
		Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position,targetRange);
		int i = 0;
		while (i < hitColliders.Length) {
			if (hitColliders [i].gameObject.tag.Contains (enemyID) &&
			    weaponsInRange.Contains (hitColliders [i].gameObject)) {
				weaponsInRange.Add (hitColliders [i].gameObject);
			}
			i++;
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
