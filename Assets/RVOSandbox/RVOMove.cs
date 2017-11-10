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

	float r_ab = 1;
	VO[] vos; //length: targets in range
	VO[,] all_rvos; //length: test velocities //length2: targets in range

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

	void GenerateVOs () {
		vos = new VO[targetsInRange.Count];
		for (int i = 0; i < targetsInRange.Count; i++) {
			RVOMove b_RVOMove = targetsInRange[i].GetComponent <RVOMove> ();
			Vector3 pos_b = b_RVOMove.pos_a;
			Vector3 vel_b = b_RVOMove.vel_a;
			float d = (pos_b - pos_a).magnitude;
			float h = (float)Math.Sqrt (r_ab * r_ab + d * d);
			Vector3 l_bound = new Vector3(-(h * r_ab) / d, 0, -(h * h) / d);
			Vector3 r_bound = -1 * l_bound;
			Vector3 vertex = pos_a + vel_b;
			float factor = (d - r_ab) / d;
			Vector3 tan_base_l = pos_a + factor * l_bound;
			Vector3 tan_base_r = pos_a + factor * r_bound;
			vos[i] = new VO (l_bound, r_bound, vertex, tan_base_l, tan_base_r - tan_base_l, vel_b);
		}
	}

	//initialize all_rvos
	void GetRVOs (Vector3 testVelocity, int i) {
		for (int j = 0; j < vos.Length; j++) {
			VO rvo = vos[j];
			rvo.vertex = .5f * (rvo.vertex + testVelocity);
			all_rvos[i,j] = rvo;
		}
	}

	//returns t of self (self is p1 + t * v1)
	tIntersect IntersectRays (Vector3 p1, Vector3 v1, Vector3 p2, Vector3 v2) {
		double det = v1.x * v2.z - v1.z * v2.x;
		if (det == 0)
			return new tIntersect(1/0.0, 1/0.0);
		double dx = p1.x - p2.x;
		double dz = p1.z - p2.z;
		double t_a = (dz * v2.x - dx * v2.z) / det;
		double t_b = (dz * v1.x - dx * v1.z) / det;
		return new tIntersect (t_a, t_b);
	}

	//NEEDS FIXING, esp for chcking if test velocity is valid (will collide)
	double GetBestTimeOnRVOs(Vector3 pos, Vector3 vel, int i) {
		List<double> possible_t = new List<double> ();
		possible_t.Add (1d); //max possible value of t
		double low_bound = 0;
		for (int j = 0; j < all_rvos.GetLength(1); j++) {
			tIntersect intersection_l = IntersectRays (pos, vel, all_rvos[i,j].vertex, all_rvos[i,j].l_bound);
			tIntersect intersection_r = IntersectRays (pos, vel, all_rvos[i,j].vertex, all_rvos[i,j].r_bound);
			double t_al = intersection_l.t1;
			double t_ar = intersection_r.t1;
			double t_la = intersection_l.t2;
			double t_ra = intersection_r.t2;

			//case1
			if (t_al > 0 && t_ar > 0 && t_la > 0 && t_ra > 0) {
				possible_t.Add (Math.Min (t_al, t_ar));
			}
			//case2+5
			else if ((t_al > 0 || t_ar > 0) && (t_la > 0 != t_ra > 0)) {
				possible_t.Add (Math.Max (t_al, t_ar));
			}
			//case4
			else if (t_al < 0 != t_ar < 0 && t_la > 0 && t_ra > 0) {
				double temp = Math.Max (t_al, t_ar);
				if (temp > low_bound)
					low_bound = temp;
			}
			//case8
			else if (t_al < 0 && t_ar < 0 && (t_la > 0 != t_ra > 0)) {
				possible_t.Add(-1);
			}
		}
		double t_min = possible_t.Min ();
		if (t_min < low_bound)
			return 0;
		else
			return t_min;
	}

	double GetMinTimeToCollision(Vector3 test_velocity, int i) {
		double smallest_t = double.MaxValue;
		for (int j = 0; j < targetsInRange.Count; j++) {
			RVOMove b_RVOMove = targetsInRange[j].GetComponent <RVOMove> ();
			Vector3 pos_b = b_RVOMove.pos_a;
			Vector3 vel_b = b_RVOMove.vel_a;

			tIntersect intersection = IntersectRays (pos_a, test_velocity - vel_b, all_rvos[i,j].tan_base_anchor, all_rvos[i,j].tan_base);
			if (intersection.t1 > 0 && intersection.t2 > 0 && intersection.t2 < 1) {
				if (intersection.t1 < smallest_t)
					smallest_t = intersection.t1;
			}
		}
		
		if (smallest_t == double.MaxValue)
			return -1;
		else
			return smallest_t;
	}

	struct tIntersect {
		public double t1;
		public double t2;

		public tIntersect(double t_1, double t_2){
			t1 = t_1;
			t2 = t_2;
		}
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

	struct VO {
		public Vector3 l_bound;
		public Vector3 r_bound;
		public Vector3 vertex;
		public Vector3 tan_base_anchor;
		public Vector3 tan_base;
		public Vector3 vel_b;

		public VO(Vector3 l, Vector3 r, Vector3 v, Vector3 tba, Vector3 tb, Vector3 vb) {
			l_bound = l;
			r_bound = r;
			vertex = v;
			tan_base_anchor = tba;
			tan_base = tb;
			vel_b = vb;
		}
	}

	Vector3 EstimateOptimalNewVelocity () {
		if (targetsInRange == null)
			return vel_a_max;
			
		Penalty[] penalties = new Penalty[NUM_TEST_VELOCITIES];
		double bestTGivenRVOs = 1;
		int indexOfBestT = 0;

		all_rvos = new VO[NUM_TEST_VELOCITIES,targetsInRange.Count];
		GenerateVOs ();

//		//If no potential collisions
//		if (GetMinTimeToCollision (vel_a_max) == -1) {
//			newPenalty = 0; 
//			return vel_a_max;
//		}

		for (int i = 0; i < testVelocities.Length; i++) {
			GetRVOs (testVelocities [i], i);
			double temp = GetBestTimeOnRVOs (pos_a, testVelocities[i], i); //gets best t value
			if (temp < 0)
				continue;
			if (temp < bestTGivenRVOs) {
				bestTGivenRVOs = temp;
				indexOfBestT = i;
			}
		}

		if (bestTGivenRVOs != 0)
			return pos_a + testVelocities[indexOfBestT];

		Debug.Log ("Covered by RVOs");

		for (int i = 0; i < testVelocities.Length; i++) {
			double tc = -1d;
			double penalty_tc = 0d;
			double penalty_stray = 0d;

			tc = GetMinTimeToCollision (testVelocities [i], i);

			if (tc > 0)
				penalty_tc = w / tc;

			//Vector3.Scale(getDirection(), vel_a_max)
			penalty_stray = (vel_a_max - testVelocities [i]).magnitude;
			penalties [i] = new Penalty(penalty_tc + penalty_stray, penalty_tc, penalty_stray);
		}

		double? minVal = null;
		int index = -1;
		if (penalties != null && penalties.Length > 0) {
			for (int i = 0; i < penalties.Length; i++) {
				double thisNum = penalties [i].penalty;
				if (!minVal.HasValue || thisNum < minVal.Value) {
					minVal = thisNum;
					index = i;
				}
			}
		}

		Debug.DrawRay (transform.position, testVelocities [index], Color.green);
		return testVelocities [index];
	}

	void Update () {
		pos_a = transform.position;

		Vector3 distanceToGoal = goal.position - transform.position;
		if (distanceToGoal.sqrMagnitude > .01) {
			vel_a_max = Vector3.Normalize(distanceToGoal) * (float)speed_a_max;
			GetInRange ();

			Vector3 step;
			if (targetsInRange.Count > 0 && targetsInRange != null) {
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
