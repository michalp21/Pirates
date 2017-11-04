using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class RVOMove : MonoBehaviour {
	public int NUM_TEST_VELOCITIES = 50;
	Vector3[] testVelocities;
	public Transform goal;

	public Vector3 pos_a_prev;
	public Vector3 pos_a;
	public double speed_a_max;
	public Vector3 vel_a_max;	//set this
	public Vector3 vel_a;
	
	double newPenalty;
	Collider[] hitColliders;
	List<GameObject> targetsInRange = new List<GameObject>(); //DS
	const int GET_TARGET_RANGE = 8;
	double w = 10; //aggressiveness

	void Start () {
		newPenalty = double.MaxValue;
		pos_a = transform.position;
		pos_a_prev = transform.position - vel_a_max * Time.deltaTime;
		speed_a_max = vel_a_max.magnitude;
		vel_a = vel_a_max;
	}

	Vector3 getDirections () {
		Vector3 diff = goal.position - this.transform.position;
		float dist = diff.magnitude;
		return diff / dist;
	}

	void GetInRange() {
		//assumes only other cylinders exist
		hitColliders = Physics.OverlapSphere (gameObject.transform.position, GET_TARGET_RANGE);
		if (hitColliders != null)
			targetsInRange = hitColliders.Where(c => c.gameObject != gameObject).Select (c => c.gameObject).ToList ();
		else
			targetsInRange = null;
	}

	//HERERE
	//ONLY GenerateTESTVELOCITIES when on collision course (when tc of current vel_a is not infinity)
	//Check out when the tc of generated test vels are nonzero

	void GenerateTestVelocities () {
		testVelocities = new Vector3[NUM_TEST_VELOCITIES];
		for (int i = 0; i < testVelocities.Length; i++) {
			float x = Random.Range (-(float)speed_a_max, (float)speed_a_max);
			float rand_z = (float)Math.Sqrt (vel_a_max.sqrMagnitude - Math.Pow(x,2));
			//Below two lines take away ability to slow down. Only direction changes.
			//float[] rand_zs = new float[]{ -rand_z, rand_z };
			//float z = rand_zs[Random.Range (0,2)];
			float z = Random.Range(-rand_z,rand_z);
			Vector3 v = new Vector3 (x,0,z);
			//Debug.DrawRay(transform.position, v, Color.green);
			testVelocities [i] = v;
		}
	}

	double GetMinTimeToCollision(Vector3 vel_a_prime) {
		double[] timesToCollision = new double[targetsInRange.Count];
		if (targetsInRange == null || targetsInRange.Count == 0)
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
		
		double[] positiveTimes = timesToCollision.Where (t => t > 0).ToArray();
		if (positiveTimes != null && positiveTimes.Length > 0)
			return positiveTimes.Min ();
		else
			return -1d;
	}

	struct Penalty {
		public double penalty;
		public double penalty_tc;
		public double penalty_stray;

		public Penalty(double p, double tc, double s) {
			penalty = p;
			penalty_tc = tc;
			penalty_stray = s;
		}
	}

	Vector3 EstimateOptimalNewVelocity () {
		Penalty[] penalties = new Penalty[NUM_TEST_VELOCITIES];
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
			penalty_stray = (vel_a_max - testVelocities [i]).magnitude;
			penalties [i] = new Penalty(penalty_tc + penalty_stray, penalty_tc, penalty_stray);
		}

		//a bit unnecessary
		Penalty[] penalties_noCollide = penalties.Where (p => p.penalty_tc == 0).ToArray();
		double? minVal = null; //nullable so this works even if you have all super-low negatives
		int index = -1;
		if (penalties_noCollide != null && penalties_noCollide.Length > 0) {
			for (int i = 0; i < penalties.Length; i++) {
				double thisNum = 0;
				if (penalties [i].penalty_tc == 0) {
					thisNum = penalties [i].penalty_stray;
				}
				else
					continue;
				if (!minVal.HasValue || thisNum < minVal.Value) {
					minVal = thisNum;
					index = i;
				}
			}
		} else {
			Debug.Log ("noCollide empty");
			for (int i = 0; i < penalties.Length; i++) {
				double thisNum = penalties [i].penalty;
				if (!minVal.HasValue || thisNum < minVal.Value) {
					minVal = thisNum;
					index = i;
				}
			}
		}
		//WRONG INDEX (of penalties_noCollide)
		Debug.DrawRay (transform.position, testVelocities [index], Color.green);
		//if on collision course
		if (GetMinTimeToCollision (vel_a) != -1) {
			newPenalty = penalties [index].penalty; 
			return testVelocities [index];
		}
		if (GetMinTimeToCollision (vel_a_max) == -1) {
			newPenalty = 0; 
			return vel_a_max;
		}
		//Decide whether or not to comment this stuff
		if (penalties [index].penalty < newPenalty) {
			newPenalty = penalties [index].penalty; 
			return testVelocities [index];
		}
		else
			return vel_a;
	}

	void Update () {
		pos_a = transform.position;

		Vector3 distanceToGoal = goal.position - transform.position;
		if (distanceToGoal.sqrMagnitude > .01) {
			vel_a_max = Vector3.Normalize(distanceToGoal) * (float)speed_a_max;
			GetInRange ();

			Vector3 step;
			if (targetsInRange.Count > 0) {
				GenerateTestVelocities ();
				vel_a = EstimateOptimalNewVelocity ();
			} else
				vel_a = vel_a_max;

			step = vel_a * Time.deltaTime;

			pos_a_prev = transform.position;
			transform.position += step;
		}
	}
}
