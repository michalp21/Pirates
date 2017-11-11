using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class RVOMove : MonoBehaviour {
	//from RVO2 Library

	//this one is for the other moving gameobjects
	public List<KeyValuePair<float, RVOMove>> neighbors = new List<KeyValuePair<float, RVOMove>>();
	
	//this one is for static obstacles in the game, not sure how we're going to implement it
	public List<KeyValuePair<float, RVOMove>> obstacles = new List<KeyValuePair<float, RVOMove>>();
	
	public List<Line> orcaLines = new List<Line>();

	public Vector3 pos_a;
	public Vector3 pref_v;
	public Vector3 vel_a;
	public Vector3 new_vel;
	
	public float maxNeighbors = 0;
	public float neighborDist = 0.0f;
	public float max_speed;
	public float radius;

	public float timeHorizon = 0.0f;
	public float timeHorizonObstacles = 0.0f;

	private KDTree tree;

	private Boolean checking = false;

	void Start () {
		//newPenalty = double.MaxValue;
		pos_a = transform.position;
		//pos_a_prev = transform.position - vel_a_max * Time.deltaTime;
		//speed_a_max = vel_a_max.magnitude;
		//vel_a = vel_a_max;
		tree = (KDTree) GameObject.FindObjectOfType (typeof(KDTree));
	}

	void Update () {
		pos_a = transform.position;

		Vector3 distance_to_goal = goal.position - pos_a;
		if(RVOMath.absSq(distance_to_goal) > 1.0f){
			distance_to_goal = RVOMath.normalize (distance_to_goal);
		}
		pref_v = distance_to_goal;

		computeNeighbors ();
		computeNewVelocity ();

		//Debug.Log (neighbors.Count());

		vel_a = new_vel;
		Vector3 step = RVOMath.spefScale(vel_a, Time.deltaTime);

		transform.position += step;

	}

	public struct Line{
		public Vector3 direction;
		public Vector3 point;
	}

	public void computeNeighbors(){
		//obstacle stuff
		//obstacles.Clear();
		//float rangeSq = RVOMath.sqr(timeHorizonObstacles * maxSpeed + radius);
		//tree.computeObstacleNeighbors(this, rangeSq);

		float rangeSq;

		neighbors.Clear();

		if(maxNeighbors > 0){
			rangeSq = RVOMath.sqr(neighborDist);
			tree.computeObjectNeighbors(this, ref rangeSq);
		}
	}

	public void computeNewVelocity(){
		orcaLines.Clear();

		// float invTimeHorizonObst = 1.0f / timeHorizonObstacles;

		// for (int i = 0; i < obstacleNeighbors_.Count; ++i)
        //     {

        //         Obstacle obstacle1 = obstacleNeighbors_[i].Value;
        //         Obstacle obstacle2 = obstacle1.next_;

        //         Vector2 relativePosition1 = obstacle1.point_ - position_;
        //         Vector2 relativePosition2 = obstacle2.point_ - position_;

        //         /*
        //          * Check if velocity obstacle of obstacle is already taken care
        //          * of by previously constructed obstacle ORCA lines.
        //          */
        //         bool alreadyCovered = false;

        //         for (int j = 0; j < orcaLines_.Count; ++j)
        //         {
        //             if (RVOMath.det(invTimeHorizonObst * relativePosition1 - orcaLines_[j].point, orcaLines_[j].direction) - invTimeHorizonObst * radius_ >= -RVOMath.RVO_EPSILON && RVOMath.det(invTimeHorizonObst * relativePosition2 - orcaLines_[j].point, orcaLines_[j].direction) - invTimeHorizonObst * radius_ >= -RVOMath.RVO_EPSILON)
        //             {
        //                 alreadyCovered = true;

        //                 break;
        //             }
        //         }

        //         if (alreadyCovered)
        //         {
        //             continue;
        //         }

        //         /* Not yet covered. Check for collisions. */
        //         float distSq1 = RVOMath.absSq(relativePosition1);
        //         float distSq2 = RVOMath.absSq(relativePosition2);

        //         float radiusSq = RVOMath.sqr(radius_);

        //         Vector2 obstacleVector = obstacle2.point_ - obstacle1.point_;
        //         float s = (-relativePosition1 * obstacleVector) / RVOMath.absSq(obstacleVector);
        //         float distSqLine = RVOMath.absSq(-relativePosition1 - s * obstacleVector);

        //         Line line;

        //         if (s < 0.0f && distSq1 <= radiusSq)
        //         {
        //             /* Collision with left vertex. Ignore if non-convex. */
        //             if (obstacle1.convex_)
        //             {
        //                 line.point = new Vector2(0.0f, 0.0f);
        //                 line.direction = RVOMath.normalize(new Vector2(-relativePosition1.y(), relativePosition1.x()));
        //                 orcaLines_.Add(line);
        //             }

        //             continue;
        //         }
        //         else if (s > 1.0f && distSq2 <= radiusSq)
        //         {
        //             /*
        //              * Collision with right vertex. Ignore if non-convex or if
        //              * it will be taken care of by neighboring obstacle.
        //              */
        //             if (obstacle2.convex_ && RVOMath.det(relativePosition2, obstacle2.direction_) >= 0.0f)
        //             {
        //                 line.point = new Vector2(0.0f, 0.0f);
        //                 line.direction = RVOMath.normalize(new Vector2(-relativePosition2.y(), relativePosition2.x()));
        //                 orcaLines_.Add(line);
        //             }

        //             continue;
        //         }
        //         else if (s >= 0.0f && s < 1.0f && distSqLine <= radiusSq)
        //         {
        //             /* Collision with obstacle segment. */
        //             line.point = new Vector2(0.0f, 0.0f);
        //             line.direction = -obstacle1.direction_;
        //             orcaLines_.Add(line);

        //             continue;
        //         }

        //         /*
        //          * No collision. Compute legs. When obliquely viewed, both legs
        //          * can come from a single vertex. Legs extend cut-off line when
        //          * non-convex vertex.
        //          */

        //         Vector2 leftLegDirection, rightLegDirection;

        //         if (s < 0.0f && distSqLine <= radiusSq)
        //         {
        //             /*
        //              * Obstacle viewed obliquely so that left vertex
        //              * defines velocity obstacle.
        //              */
        //             if (!obstacle1.convex_)
        //             {
        //                 /* Ignore obstacle. */
        //                 continue;
        //             }

        //             obstacle2 = obstacle1;

        //             float leg1 = RVOMath.sqrt(distSq1 - radiusSq);
        //             leftLegDirection = new Vector2(relativePosition1.x() * leg1 - relativePosition1.y() * radius_, relativePosition1.x() * radius_ + relativePosition1.y() * leg1) / distSq1;
        //             rightLegDirection = new Vector2(relativePosition1.x() * leg1 + relativePosition1.y() * radius_, -relativePosition1.x() * radius_ + relativePosition1.y() * leg1) / distSq1;
        //         }
        //         else if (s > 1.0f && distSqLine <= radiusSq)
        //         {
        //             /*
        //              * Obstacle viewed obliquely so that
        //              * right vertex defines velocity obstacle.
        //              */
        //             if (!obstacle2.convex_)
        //             {
        //                 /* Ignore obstacle. */
        //                 continue;
        //             }

        //             obstacle1 = obstacle2;

        //             float leg2 = RVOMath.sqrt(distSq2 - radiusSq);
        //             leftLegDirection = new Vector2(relativePosition2.x() * leg2 - relativePosition2.y() * radius_, relativePosition2.x() * radius_ + relativePosition2.y() * leg2) / distSq2;
        //             rightLegDirection = new Vector2(relativePosition2.x() * leg2 + relativePosition2.y() * radius_, -relativePosition2.x() * radius_ + relativePosition2.y() * leg2) / distSq2;
        //         }
        //         else
        //         {
        //             /* Usual situation. */
        //             if (obstacle1.convex_)
        //             {
        //                 float leg1 = RVOMath.sqrt(distSq1 - radiusSq);
        //                 leftLegDirection = new Vector2(relativePosition1.x() * leg1 - relativePosition1.y() * radius_, relativePosition1.x() * radius_ + relativePosition1.y() * leg1) / distSq1;
        //             }
        //             else
        //             {
        //                 /* Left vertex non-convex; left leg extends cut-off line. */
        //                 leftLegDirection = -obstacle1.direction_;
        //             }

        //             if (obstacle2.convex_)
        //             {
        //                 float leg2 = RVOMath.sqrt(distSq2 - radiusSq);
        //                 rightLegDirection = new Vector2(relativePosition2.x() * leg2 + relativePosition2.y() * radius_, -relativePosition2.x() * radius_ + relativePosition2.y() * leg2) / distSq2;
        //             }
        //             else
        //             {
        //                 /* Right vertex non-convex; right leg extends cut-off line. */
        //                 rightLegDirection = obstacle1.direction_;
        //             }
        //         }

        //         /*
        //          * Legs can never point into neighboring edge when convex
        //          * vertex, take cutoff-line of neighboring edge instead. If
        //          * velocity projected on "foreign" leg, no constraint is added.
        //          */

        //         Obstacle leftNeighbor = obstacle1.previous_;

        //         bool isLeftLegForeign = false;
        //         bool isRightLegForeign = false;

        //         if (obstacle1.convex_ && RVOMath.det(leftLegDirection, -leftNeighbor.direction_) >= 0.0f)
        //         {
        //             /* Left leg points into obstacle. */
        //             leftLegDirection = -leftNeighbor.direction_;
        //             isLeftLegForeign = true;
        //         }

        //         if (obstacle2.convex_ && RVOMath.det(rightLegDirection, obstacle2.direction_) <= 0.0f)
        //         {
        //             /* Right leg points into obstacle. */
        //             rightLegDirection = obstacle2.direction_;
        //             isRightLegForeign = true;
        //         }

        //         /* Compute cut-off centers. */
        //         Vector2 leftCutOff = invTimeHorizonObst * (obstacle1.point_ - position_);
        //         Vector2 rightCutOff = invTimeHorizonObst * (obstacle2.point_ - position_);
        //         Vector2 cutOffVector = rightCutOff - leftCutOff;

        //         /* Project current velocity on velocity obstacle. */

        //         /* Check if current velocity is projected on cutoff circles. */
        //         float t = obstacle1 == obstacle2 ? 0.5f : ((velocity_ - leftCutOff) * cutOffVector) / RVOMath.absSq(cutOffVector);
        //         float tLeft = (velocity_ - leftCutOff) * leftLegDirection;
        //         float tRight = (velocity_ - rightCutOff) * rightLegDirection;

        //         if ((t < 0.0f && tLeft < 0.0f) || (obstacle1 == obstacle2 && tLeft < 0.0f && tRight < 0.0f))
        //         {
        //             /* Project on left cut-off circle. */
        //             Vector2 unitW = RVOMath.normalize(velocity_ - leftCutOff);

        //             line.direction = new Vector2(unitW.y(), -unitW.x());
        //             line.point = leftCutOff + radius_ * invTimeHorizonObst * unitW;
        //             orcaLines_.Add(line);

        //             continue;
        //         }
        //         else if (t > 1.0f && tRight < 0.0f)
        //         {
        //             /* Project on right cut-off circle. */
        //             Vector2 unitW = RVOMath.normalize(velocity_ - rightCutOff);

        //             line.direction = new Vector2(unitW.y(), -unitW.x());
        //             line.point = rightCutOff + radius_ * invTimeHorizonObst * unitW;
        //             orcaLines_.Add(line);

        //             continue;
        //         }

        //         /*
        //          * Project on left leg, right leg, or cut-off line, whichever is
        //          * closest to velocity.
        //          */
        //         float distSqCutoff = (t < 0.0f || t > 1.0f || obstacle1 == obstacle2) ? float.PositiveInfinity : RVOMath.absSq(velocity_ - (leftCutOff + t * cutOffVector));
        //         float distSqLeft = tLeft < 0.0f ? float.PositiveInfinity : RVOMath.absSq(velocity_ - (leftCutOff + tLeft * leftLegDirection));
        //         float distSqRight = tRight < 0.0f ? float.PositiveInfinity : RVOMath.absSq(velocity_ - (rightCutOff + tRight * rightLegDirection));

        //         if (distSqCutoff <= distSqLeft && distSqCutoff <= distSqRight)
        //         {
        //             /* Project on cut-off line. */
        //             line.direction = -obstacle1.direction_;
        //             line.point = leftCutOff + radius_ * invTimeHorizonObst * new Vector2(-line.direction.y(), line.direction.x());
        //             orcaLines_.Add(line);

        //             continue;
        //         }

        //         if (distSqLeft <= distSqRight)
        //         {
        //             /* Project on left leg. */
        //             if (isLeftLegForeign)
        //             {
        //                 continue;
        //             }

        //             line.direction = leftLegDirection;
        //             line.point = leftCutOff + radius_ * invTimeHorizonObst * new Vector2(-line.direction.y(), line.direction.x());
        //             orcaLines_.Add(line);

        //             continue;
        //         }

        //         /* Project on right leg. */
        //         if (isRightLegForeign)
        //         {
        //             continue;
        //         }

        //         line.direction = -rightLegDirection;
        //         line.point = rightCutOff + radius_ * invTimeHorizonObst * new Vector2(-line.direction.y(), line.direction.x());
        //         orcaLines_.Add(line);
        //     }

		int numObstLines = orcaLines.Count();

		//Debug.Log (neighbors.Count());

		float invTimeHorizon = 1.0f/timeHorizon;

		for(int i = 0; i < neighbors.Count(); ++i){
			RVOMove other = neighbors[i].Value;

			Vector3 rel_pos = other.pos_a - pos_a;
			Vector3 rel_vel = vel_a - other.vel_a;

			float distSq = RVOMath.absSq(rel_pos);
			float combinedRadius = radius + other.radius;
			float combinedRadiusSq = RVOMath.sqr(combinedRadius);

			Line line;
			Vector3 u;

			//Debug.Log ("distSq: " + distSq + ", combinedRS: " + combinedRadiusSq);

			if(distSq > combinedRadiusSq){
				Vector3 w = rel_vel - RVOMath.spefScale(rel_pos, invTimeHorizon);

				float wLengthSq = RVOMath.absSq(w);
				float dotProduct1 = Vector3.Dot (w, rel_pos);

				if(dotProduct1 < 0.0f && RVOMath.sqr(dotProduct1) > combinedRadiusSq * wLengthSq){
					float wLength = RVOMath.sqrt(wLengthSq);
					Vector3 unitW = w / wLength;

					line.direction = new Vector3(unitW.z, 0, -unitW.x);
					u = RVOMath.spefScale (unitW, (combinedRadius * invTimeHorizon - wLength));
				}else{
					float leg = RVOMath.sqrt(distSq - combinedRadiusSq);

					if(RVOMath.det(rel_pos, w) > 0.0f){
						line.direction = new Vector3(rel_pos.x * leg - rel_pos.z *combinedRadius, 0,rel_pos.x * combinedRadius + rel_pos.z * leg) / distSq;
					}else{
						line.direction = -new Vector3(rel_pos.x * leg + rel_pos.z * combinedRadius, 0, -rel_pos.x * combinedRadius + rel_pos.z * leg) / distSq;
					}

					float dotProduct2 = Vector3.Dot(rel_vel, line.direction);
					//COMEBACKPLS
					u = RVOMath.spefScale(line.direction, dotProduct2) - rel_vel;
					//u = dotProduct2 * line.direction - rel_vel;
				}
			}else{
				float invTimeStep = 1.0f / Time.fixedDeltaTime;

				Vector3 w = rel_vel - RVOMath.spefScale (rel_pos, invTimeStep);
				//Debug.Log ("w--- " + w);
				
				float wLength = RVOMath.abs(w);
				Vector3 unitW = RVOMath.spefDiv (w, wLength);

				line.direction = new Vector3(unitW.z, 0, -unitW.x);
				u = RVOMath.spefScale(unitW, (combinedRadius * invTimeStep - wLength));
			}

			line.point = vel_a + RVOMath.spefScale(u, 0.5f);
			orcaLines.Add(line);


		}

		foreach(Line x in orcaLines){
			//Debug.Log (x.point);
		}

		int lineFail = linearProgram2(orcaLines, max_speed, pref_v, false, ref new_vel);

		if (checking) {
			//Debug.Log ("FROM LINEFAIL");
		}
		//Debug.Log ("f: " + lineFail);

		if(lineFail < orcaLines.Count()){
			linearProgram3(orcaLines, numObstLines, lineFail, max_speed, ref new_vel);
			if (checking) {
				//Debug.Log ("LINEAR3");
			}
		}

		

	}

	internal void insertObjectNeighbor(RVOMove o, ref float rangeSq){
		if(this != o){
			float distSq = RVOMath.absSq(pos_a - o.pos_a);

			if(distSq < rangeSq){
				if(neighbors.Count() < maxNeighbors){
					neighbors.Add(new KeyValuePair<float, RVOMove>(distSq, o));
				}

				int i = neighbors.Count() - 1;

				while(i != 0 && distSq < neighbors[i-1].Key){
					neighbors[i] = neighbors[i-1];
					--i;
				}

				neighbors[i] = new KeyValuePair<float, RVOMove>(distSq, o);

				if(neighbors.Count() == maxNeighbors){
					rangeSq = neighbors[neighbors.Count() - 1].Key;
				}
			}
		}
	}

	private bool linearProgram1(List<Line> lines, int lineNo, float radius, Vector3 opt_vel, bool dir_opt, ref Vector3 result){
		//Debug.Log ("Before Line1: " + pos_a.y);
		float dotProduct = Vector3.Dot(lines[lineNo].point, lines[lineNo].direction);
		float discriminant = RVOMath.sqr(dotProduct) + RVOMath.sqr(radius) - RVOMath.absSq(lines[lineNo].point);

		if(discriminant < 0.0f){
			return false;
		}

		float sqrtDiscrim = RVOMath.sqrt(discriminant);
		float tLeft = -dotProduct - sqrtDiscrim;
		float tRight = -dotProduct + sqrtDiscrim;

		for(int i = 0; i < lineNo; ++i){
			float denom = RVOMath.det(lines[lineNo].direction, lines[i].direction);
			float numr = RVOMath.det(lines[i].direction, lines[lineNo].point - lines[i].point);

			if(RVOMath.fabs(denom) <= RVOMath.RVO_EPSILON){
				if(numr < 0.0f){
					return false;
				}

				continue;
			}

			float t = numr/denom;

			if(denom >= 0.0f){
				tRight = Math.Min(tRight, t);
			}else{
				tLeft = Math.Max(tLeft, t);
			}

			if(tLeft > tRight){
				return false;
			}
		}

		if(dir_opt){
			if(Vector3.Dot(opt_vel, lines[lineNo].direction) > 0.0f){
				result = lines [lineNo].point + RVOMath.spefScale (lines [lineNo].direction, tRight);
					
			}else{
				result = lines[lineNo].point + RVOMath.spefScale (lines [lineNo].direction, tLeft);
			}
		}else{
			
			float t = Vector3.Dot(lines[lineNo].direction, (opt_vel - lines[lineNo].point));

			if(t < tLeft){
				result = lines[lineNo].point + RVOMath.spefScale (lines [lineNo].direction, tLeft);
			}else if(t > tRight){
				result = lines [lineNo].point + RVOMath.spefScale (lines [lineNo].direction, tRight);
			}else{
				result = lines[lineNo].point + RVOMath.spefScale (lines [lineNo].direction, t);
			}
		}
		//Debug.Log ("After Line1: " + pos_a.y);
		return true;
	}

	private int linearProgram2(List<Line> lines, float radius, Vector3 opt_vel, bool dir_opt, ref Vector3 result){
		//Debug.Log ("Before Line2: " + pos_a.y);
		if(dir_opt){
			result = RVOMath.spefScale(opt_vel, radius);
		}else if(RVOMath.absSq(opt_vel) > RVOMath.sqr(radius)){
			result = RVOMath.spefScale(RVOMath.normalize(opt_vel), radius);
		}else{
			result = opt_vel;
		}

		for(int i = 0; i < lines.Count(); ++i){
			if(RVOMath.det(lines[i].direction, lines[i].point - result) > 0.0f){
				Vector3 temp = result;
				if(!linearProgram1(lines, i, radius, opt_vel, dir_opt, ref result)){
					result = temp;
					return i;
				}
			}
		}
		//Debug.Log ("After Line2: " + pos_a.y);
		return lines.Count();
	}

	private void linearProgram3(List<Line> lines, int numObstLines, int beginLine, float radius, ref Vector3 result){
		//Debug.Log ("Before Line3: " + pos_a.y);
		float distance = 0.0f;

		for(int i = beginLine; i < lines.Count(); ++i){
			if(RVOMath.det(lines[i].direction, lines[i].point - result) > distance){
				List<Line> projLines = new List<Line>();
				for(int ii = 0; ii < numObstLines; ++ii){
					projLines.Add(lines[ii]);
				}

				for(int j = numObstLines; j < i; ++j){
					Line line;
					float determinant = RVOMath.det(lines[i].direction, lines[j].direction);

					if(RVOMath.fabs(determinant) <= RVOMath.RVO_EPSILON){
						if(Vector3.Dot(lines[i].direction, lines[j].direction) > 0.0f){
							continue;
						}else{
							line.point = RVOMath.spefScale((lines[i].point + lines[j].point), 0.5f);
						}
					}else{
						line.point = lines[i].point + RVOMath.spefScale(lines[i].direction, (RVOMath.det(lines[j].direction, lines[i].point - lines[j].point) /determinant));
					}

					line.direction = RVOMath.normalize(lines[j].direction - lines[i].direction);
					projLines.Add(line);
				}

				Vector3 temp = result;
				if(linearProgram2(projLines, radius, new Vector3(-lines[i].direction.z, 0, lines[i].direction.x), true, ref result) < projLines.Count()){
					result = temp;
				}

				distance = RVOMath.det(lines[i].direction, lines[i].point - result);
			}
		}
		//Debug.Log ("After Line3: " + pos_a.y);
	}

	// public int NUM_TEST_VELOCITIES = 50;
	// Vector3[] testVelocities;
	public Transform goal;

	// public Vector3 pos_a_prev;
	// public Vector3 pos_a;
	// public double speed_a_max;
	// public Vector3 vel_a_max;	//set this
	// public Vector3 vel_a;

	// double newPenalty;
	// Collider[] hitColliders;
	// List<GameObject> targetsInRange = new List<GameObject>(); //DS
	// const int GET_TARGET_RANGE = 8;
	// double w = 10; //aggressiveness

	// float r_ab = 1;
	// VO[] vos; //length: targets in range
	// VO[,] all_rvos; //length: test velocities //length2: targets in range

//	void Start () {
//		//newPenalty = double.MaxValue;
//		pos_a = transform.position;
//		//pos_a_prev = transform.position - vel_a_max * Time.deltaTime;
//		//speed_a_max = vel_a_max.magnitude;
//		//vel_a = vel_a_max;
//		tree = (KDTree) GameObject.FindObjectOfType (typeof(KDTree));
//	}

//	Vector3 getDirections () {
//		Vector3 diff = goal.position - this.transform.position;
//		float dist = diff.magnitude;
//		return diff / dist;
//	}
//
//	void GetInRange() {
//		//assumes only other cylinders exist
//		hitColliders = Physics.OverlapSphere (gameObject.transform.position, GET_TARGET_RANGE);
//		if (hitColliders != null)
//			targetsInRange = hitColliders.Where(c => c.gameObject != gameObject).Select (c => c.gameObject).ToList ();
//		else
//			targetsInRange = null;
//	}
//
//	//ONLY GenerateTESTVELOCITIES when on collision course (when tc of current vel_a is not infinity)
//	//Check out when the tc of generated test vels are nonzero
//
//	void GenerateTestVelocities () {
//		testVelocities = new Vector3[NUM_TEST_VELOCITIES];
//		for (int i = 0; i < testVelocities.Length; i++) {
//			float x = Random.Range (-(float)speed_a_max, (float)speed_a_max);
//			float rand_z = (float)Math.Sqrt (vel_a_max.sqrMagnitude - Math.Pow(x,2));
//			//Below two lines take away ability to slow down. Only direction changes.
//			//float[] rand_zs = new float[]{ -rand_z, rand_z };
//			//float z = rand_zs[Random.Range (0,2)];
//			float z = Random.Range(-rand_z,rand_z);
//			Vector3 v = new Vector3 (x,0,z);
//			//Debug.DrawRay(transform.position, v, Color.green);
//			testVelocities [i] = v;
//		}
//	}
//
//	void GenerateVOs () {
//		vos = new VO[targetsInRange.Count];
//		for (int i = 0; i < targetsInRange.Count; i++) {
//			RVOMove b_RVOMove = targetsInRange[i].GetComponent <RVOMove> ();
//			Vector3 pos_b = b_RVOMove.pos_a;
//			Vector3 vel_b = b_RVOMove.vel_a;
//			float d = (pos_b - pos_a).magnitude;
//			float h = (float)Math.Sqrt (r_ab * r_ab + d * d);
//			Vector3 l_bound = new Vector3(-(h * r_ab) / d, 0, -(h * h) / d);
//			Vector3 r_bound = -1 * l_bound;
//			Vector3 vertex = pos_a + vel_b;
//			float factor = (d - r_ab) / d;
//			Vector3 tan_base_l = pos_a + factor * l_bound;
//			Vector3 tan_base_r = pos_a + factor * r_bound;
//			vos[i] = new VO (l_bound, r_bound, vertex, tan_base_l, tan_base_r - tan_base_l, vel_b);
//		}
//	}
//
//	//initialize all_rvos
//	void GetRVOs (Vector3 testVelocity, int i) {
//		for (int j = 0; j < vos.Length; j++) {
//			VO rvo = vos[j];
//			rvo.vertex = .5f * (rvo.vertex + testVelocity);
//			all_rvos[i,j] = rvo;
//		}
//	}
//
//	//returns t of self (self is p1 + t * v1)
//	tIntersect IntersectRays (Vector3 p1, Vector3 v1, Vector3 p2, Vector3 v2) {
//		double det = v1.x * v2.z - v1.z * v2.x;
//		if (det == 0)
//			return new tIntersect(1/0.0, 1/0.0);
//		double dx = p1.x - p2.x;
//		double dz = p1.z - p2.z;
//		double t_a = (dz * v2.x - dx * v2.z) / det;
//		double t_b = (dz * v1.x - dx * v1.z) / det;
//		return new tIntersect (t_a, t_b);
//	}
//
//	//NEEDS FIXING, esp for chcking if test velocity is valid (will collide)
//	double GetBestTimeOnRVOs(Vector3 pos, Vector3 vel, int i) {
//		List<double> possible_t = new List<double> ();
//		possible_t.Add (1d); //max possible value of t
//		double low_bound = 0;
//		for (int j = 0; j < all_rvos.GetLength(1); j++) {
//			tIntersect intersection_l = IntersectRays (pos, vel, all_rvos[i,j].vertex, all_rvos[i,j].l_bound);
//			tIntersect intersection_r = IntersectRays (pos, vel, all_rvos[i,j].vertex, all_rvos[i,j].r_bound);
//			double t_al = intersection_l.t1;
//			double t_ar = intersection_r.t1;
//			double t_la = intersection_l.t2;
//			double t_ra = intersection_r.t2;
//
//			//case1
//			if (t_al > 0 && t_ar > 0 && t_la > 0 && t_ra > 0) {
//				possible_t.Add (Math.Min (t_al, t_ar));
//			}
//			//case2+5
//			else if ((t_al > 0 || t_ar > 0) && (t_la > 0 != t_ra > 0)) {
//				possible_t.Add (Math.Max (t_al, t_ar));
//			}
//			//case4
//			else if (t_al < 0 != t_ar < 0 && t_la > 0 && t_ra > 0) {
//				double temp = Math.Max (t_al, t_ar);
//				if (temp > low_bound)
//					low_bound = temp;
//			}
//			//case8
//			else if (t_al < 0 && t_ar < 0 && (t_la > 0 != t_ra > 0)) {
//				possible_t.Add(-1);
//			}
//		}
//		double t_min = possible_t.Min ();
//		if (t_min < low_bound)
//			return 0;
//		else
//			return t_min;
//	}
//
//	double GetMinTimeToCollision(Vector3 vel_a_prime) {
//		double[] timesToCollision = new double[targetsInRange.Count];
//		if (targetsInRange == null || targetsInRange.Count == 0)
//			return -1d;
//		for (int i = 0; i < targetsInRange.Count; i++) {
//			RVOMove b_RVOMove = targetsInRange[i].GetComponent <RVOMove> ();
//			Vector3 pos_b = b_RVOMove.pos_a;
//			Vector3 vel_b = b_RVOMove.vel_a;
//
//			Vector3 vel = 2 * vel_a_prime - vel_a - vel_b;
//
//			double r_ab = GetComponent<CapsuleCollider> ().radius + targetsInRange[i].GetComponent<CapsuleCollider> ().radius;
//
//			double a = Vector3.Dot(vel,vel);
//			double b = 2 * Vector3.Dot(vel, pos_a - pos_b);
//			double c = Vector3.Dot(pos_a, pos_a) + Vector3.Dot(pos_b, pos_b) - 2 * Vector3.Dot(pos_a, pos_b) - Math.Pow(r_ab, 2);
//
//			double disc = Math.Pow(b, 2) - 4 * a * c;
//			if (disc < 0) {
//				timesToCollision[i] = -1d;
//				continue;
//			}
//			double sqrt_disc = Math.Sqrt (disc);
//			timesToCollision[i] = (-b - sqrt_disc) / (2 * a);
//		}
//
//		double[] positiveTimes = timesToCollision.Where (t => t > 0).ToArray();
//		if (positiveTimes != null && positiveTimes.Length > 0)
//			return positiveTimes.Min ();
//		else
//			return -1d;
//	}
//
//	double GetMinTimeToCollision(Vector3 test_velocity, int i) {
//		double smallest_t = double.MaxValue;
//		for (int j = 0; j < targetsInRange.Count; j++) {	
//			RVOMove b_RVOMove = targetsInRange[j].GetComponent <RVOMove> ();
//			Vector3 pos_b = b_RVOMove.pos_a;
//			Vector3 vel_b = b_RVOMove.vel_a;
//
//			tIntersect intersection = IntersectRays (pos_a, test_velocity - vel_b, all_rvos[i,j].tan_base_anchor, all_rvos[i,j].tan_base);
//			if (intersection.t1 > 0 && intersection.t2 > 0 && intersection.t2 < 1) {
//				if (intersection.t1 < smallest_t)
//					smallest_t = intersection.t1;
//			}
//		}
//		
//		if (smallest_t == double.MaxValue)
//			return -1;
//		else
//			return smallest_t;
//	}
//
//	struct tIntersect {
//		public double t1;
//		public double t2;
//
//		public tIntersect(double t_1, double t_2){
//			t1 = t_1;
//			t2 = t_2;
//		}
//	}
//
//	struct Penalty {
//		public double penalty;
//		public double penalty_tc;
//		public double penalty_stray;
//
//		public Penalty(double p, double tc, double s) {
//			penalty = p;
//			penalty_tc = tc;
//			penalty_stray = s;
//		}
//	}
//
//	struct VO {
//		public Vector3 l_bound;
//		public Vector3 r_bound;
//		public Vector3 vertex;
//		public Vector3 tan_base_anchor;
//		public Vector3 tan_base;
//		public Vector3 vel_b;
//
//		public VO(Vector3 l, Vector3 r, Vector3 v, Vector3 tba, Vector3 tb, Vector3 vb) {
//			l_bound = l;
//			r_bound = r;
//			vertex = v;
//			tan_base_anchor = tba;
//			tan_base = tb;
//			vel_b = vb;
//		}
//	}
//
//	Vector3 EstimateOptimalNewVelocity () {
//		if (GetMinTimeToCollision (vel_a_max) == -1) {
//			newPenalty = 0; 
//			return vel_a_max;
//		}
//		if (GetMinTimeToCollision (vel_a) == -1) {
//			newPenalty = 0;
//			return vel_a;
//		}
//		double bestTGivenRVOs = 1;
//		int indexOfBestT = 0;
//
//		all_rvos = new VO[NUM_TEST_VELOCITIES,targetsInRange.Count];
//		GenerateVOs ();
//
//			//		//If no potential collisions
//			//		if (GetMinTimeToCollision (vel_a_max) == -1) {
//			//			newPenalty = 0; 
//			//			return vel_a_max;
//			//		}
////
////		for (int i = 0; i < testVelocities.Length; i++) {
////			GetRVOs (testVelocities [i], i);
////			double temp = GetBestTimeOnRVOs (pos_a, testVelocities[i], i); //gets best t value
////			if (temp < 0)
////				continue;
////			if (temp < bestTGivenRVOs) {
////				bestTGivenRVOs = temp;
////				indexOfBestT = i;
////			}
////		}
////
////		if (bestTGivenRVOs != 0)
////			return pos_a + testVelocities[indexOfBestT];
//
//			//Debug.Log ("Covered by RVOs");
//
//		double? minVal = null;
//		int index = -1;
//
//		for (int i = 0; i < testVelocities.Length; i++) {
//			GetRVOs (testVelocities [i], i);
//			double tc = -1d;
//			double penalty_tc = 0d;
//			double penalty_stray = 0d;
//
//			tc = GetMinTimeToCollision (testVelocities [i], i);
//
//			if (tc > 0)
//				penalty_tc = w / tc;
//
//			//Vector3.Scale(getDirection(), vel_a_max)
//			penalty_stray = (vel_a_max - testVelocities [i]).magnitude;
//
//			Penalty p = new Penalty (penalty_tc + penalty_stray, penalty_tc, penalty_stray);
//
//			if (!minVal.HasValue || p.penalty < minVal.Value) {
//				minVal = p.penalty;
//				index = i;
//			}
//		}
//
//		Debug.DrawRay (transform.position, testVelocities [index], Color.green);
//
//		newPenalty = (double)minVal;
//		return testVelocities [index];
//	}
//
//	void Update () {
//		pos_a = transform.position;
//
//		Vector3 distanceToGoal = goal.position - transform.position;
//		if (distanceToGoal.sqrMagnitude > .01) {
//			vel_a_max = Vector3.Normalize(distanceToGoal) * (float)speed_a_max;
//			GetInRange ();
//
//			Vector3 step;
//			if (targetsInRange != null && targetsInRange.Count > 0) {
//				GenerateTestVelocities ();
//				vel_a = EstimateOptimalNewVelocity ();
//			} else
//				vel_a = vel_a_max;
//
//			step = vel_a * Time.deltaTime;
//
//			pos_a_prev = transform.position;
//			transform.position += step;
//		}
//	}
}
