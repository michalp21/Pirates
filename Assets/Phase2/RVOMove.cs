using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class RVOMove : MonoBehaviour {
	public int NUM_TEST_VELOCITIES = 20;
	Vector3[] testVelocities;
	public Transform goal;

	public Vector3 pos_a_prev;
	public Vector3 pos_a;
	public double speed_a_max;
	public Vector3 vel_a_max;	//set this
	public Vector3 vel_a;

	Collider[] hitColliders;
	List<GameObject> targetsInRange = new List<GameObject>(); //DS
	const int GET_TARGET_RANGE = 10;
	double w = 1; //aggressiveness

	void Start () {
		pos_a = transform.position;
		pos_a_prev = transform.position - vel_a_max * Time.deltaTime;
		speed_a_max = vel_a_max.magnitude;
	}

	void GetInRange() {
		//assumes only other cylinders exist
		hitColliders = Physics.OverlapSphere (gameObject.transform.position, GET_TARGET_RANGE);
		if (hitColliders != null)
			targetsInRange = hitColliders.Where(c => c.gameObject != gameObject).Select (c => c.gameObject).ToList ();
		else
			targetsInRange = null;
	}

	Vector3 getDirections () {
		Vector3 diff = goal.position - this.transform.position;
		float dist = diff.magnitude;
		return diff / dist;
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

//			Debug.Log ("Pos_a " + pos_a);
//			Debug.Log ("Vel_a " + vel_a);
//			Debug.Log ("Pos_b " + pos_b);
//			Debug.Log ("Vel_b " + vel_b);
//			Debug.Log ("Vel " + vel);

			double disc = Math.Pow(b, 2) - 4 * a * c;
			if (disc < 0) {
				timesToCollision[i] = -1d;
				continue;
			}
			double sqrt_disc = Math.Sqrt (disc);
//			Debug.Log ("a " + a + "    b " + b + "    c " + c);
			timesToCollision[i] = (-b - sqrt_disc) / (2 * a);
		}
		if (timesToCollision.Length > 0)
			return timesToCollision.Min ();
		else
			return -1d;
	}

	//HERERE
	//ONLY GenerateTESTVELOCITIES when on collision course (when tc of current vel_a is not infinity)
	//Check out when the tc of generated test vels are nonzero

	void GenerateTestVelocities () {
		testVelocities = new Vector3[NUM_TEST_VELOCITIES];
		for (int i = 0; i < testVelocities.Length; i++) {
			float x = Random.Range (-(float)speed_a_max, (float)speed_a_max);
			float rand_z = (float)Math.Sqrt (vel_a_max.sqrMagnitude - Math.Pow(x,2));
			float z = Random.Range (-rand_z, rand_z);
			Vector3 v = new Vector3 (x,0,z);
			Debug.DrawRay(transform.position, v, Color.green);
			testVelocities [i] = v;
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

			//Vector3.Scale(getDirection(), vel_a_max)
			penalty_stray = (vel_a_max - testVelocities[i]).magnitude;
			Debug.Log ("tc: " + penalty_tc + " stray: " + penalty_stray);
			penalties [i] = penalty_tc + penalty_stray;
		}

		double? minVal = null; //nullable so this works even if you have all super-low negatives
		int index = -1;
		for (int i = 0; i < penalties.Length; i++)
		{
			double thisNum = penalties[i];
			if (!minVal.HasValue || thisNum < minVal.Value)
			{
				minVal = thisNum;
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
//		Vector3 step = vel_a_max * Time.deltaTime;
		Debug.Log ("------- " + gameObject.name + " --------");

		pos_a_prev = transform.position;
		transform.position += step;
		//transform.position = Vector3.MoveTowards(transform.position, goal.position, step);
	}
}
