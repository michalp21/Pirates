using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class RVOMove : MonoBehaviour {
	public int NUM_TEST_VELOCITIES = 100;
	Vector3[] testVelocities;
	public Transform goal;

	public Vector3 pos_a_prev;
	public Vector3 pos_a;
	public double speed_a_max;
	public Vector3 vel_a_max;	//set this
	public Vector3 vel_a;

	Collider[] hitColliders;
	List<GameObject> targetsInRange = new List<GameObject>(); //DS
	const int GET_TARGET_RANGE = 5;
	double w = 1; //aggressiveness

	void Start () {
		pos_a = transform.position;
		pos_a_prev = transform.position;
		speed_a_max = vel_a_max.magnitude;
	}

	void GetInRange() {
		//assumes only other cylinders exist
		hitColliders = Physics.OverlapSphere (gameObject.transform.position, GET_TARGET_RANGE);
		if (hitColliders != null)
			targetsInRange = hitColliders.Select (c => c.gameObject).ToList ();
		else
			targetsInRange = null;
	}

	double GetMinTimeToCollision(Vector3 vel_a_prime) {
		double[] timesToCollision = new double[targetsInRange.Count];
		if (targetsInRange == null)
			return -1d;
		for (int i = 0; i < targetsInRange.Count; i++) {
			RVOMove b_RVOMove = targetsInRange[i].GetComponent <RVOMove> ();
			Vector3 pos_b = b_RVOMove.pos_a;
			Vector3 vel_b = b_RVOMove.vel_a;

			Vector3 vel = 2 * vel_a_prime - vel_a - vel_b;

			double r_ab = GetComponent<CapsuleCollider> ().radius + targetsInRange[i].GetComponent<CapsuleCollider> ().radius;

			double a = Vector3.Dot(vel,vel);
			double b = 2 * Vector3.Dot(vel, pos_a - pos_b);
			double c = Vector3.Dot(pos_a, pos_a) + Vector3.Dot(pos_b, pos_b) - 2 * Vector3.Dot(pos_a, pos_b) - Math.Pow(r_ab, 2);

			double disc = Math.Pow(b, 2) - 4 * a * c;
			if (disc < 0) {
				timesToCollision[i] = -1d;
				continue;
			}
			double sqrt_disc = Math.Sqrt (disc);
			timesToCollision[i] = (-b - sqrt_disc) / (2 * a);
		}
		foreach(GameObject target in targetsInRange)
		{
			Debug.Log (target.name);
		}
		Debug.Log (timesToCollision);
		timesToCollision = Array.FindAll(timesToCollision, time => time > 0);
		Debug.Log (timesToCollision);
		if (timesToCollision != null)
			return timesToCollision.Min ();
		else
			return -1d;
	}

	void GenerateTestVelocities () {
		testVelocities = new Vector3[NUM_TEST_VELOCITIES];
		for (int i = 0; i < testVelocities.Length; i++) {
			float x = Random.Range (0f, (float)speed_a_max);
			float z = Random.Range (0f, (float)Math.Sqrt (vel_a_max.sqrMagnitude - x * x));
			testVelocities [i] = new Vector3 (x,0,z);
		}
	}

	Vector3 EstimateOptimalNewVelocity () {
		double[] penalties = new double[NUM_TEST_VELOCITIES];
		//for 100+ evenly distributed test velocities //use rays colliding against rvo bounds?
		//calculate penalty for each test velocity and stick into array
		//return argmin
		for (int i = 0; i < testVelocities.Length; i++) {

			double tc = -1d;
			double penalty_tc = 0d;
			double penalty_stray = 0d;

			if (targetsInRange != null)
				tc = GetMinTimeToCollision (testVelocities [i]);
			if (tc > 0)
				penalty_tc = w / tc;

			penalty_stray = (vel_a_max - testVelocities[i]).magnitude;
			penalties [i] = penalty_tc + penalty_stray;
		}

		double? maxVal = null; //nullable so this works even if you have all super-low negatives
		int index = -1;
		for (int i = 0; i < penalties.Length; i++)
		{
			double thisNum = penalties[i];
			if (!maxVal.HasValue || thisNum > maxVal.Value)
			{
				maxVal = thisNum;
				index = i;
			}
		}
		return testVelocities [index];
	}

	void Update () {
		pos_a = transform.position;
		vel_a = (transform.position - pos_a_prev) / Time.deltaTime;
		GetInRange ();
		GenerateTestVelocities ();

		Vector3 vel_a_new = EstimateOptimalNewVelocity ();
		Vector3 step = vel_a_new * Time.deltaTime;

		pos_a_prev = transform.position;
		transform.position += step;
		//transform.position = Vector3.MoveTowards(transform.position, goal.position, step);
	}
}
