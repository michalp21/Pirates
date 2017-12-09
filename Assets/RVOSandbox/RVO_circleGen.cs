using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RVO_circleGen : MonoBehaviour {
	public int numObjects;
	public int radius;
	public GameObject prefab;
	public GameObject goal;
	List<GameObject> newObjs = new List<GameObject> ();

	Vector3 PointOnCircle(Vector3 center, float radius, float ang) {
		Vector3 pos;
		pos.x = center.x + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
		pos.y = center.y;
		pos.z = center.z + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		return pos;
	}

	//Detect overlap with light up red

	void Start () {
		Vector3 center = transform.position;
		int intervals = 360 / numObjects;
		for (int i = 0; i < 360; i+=intervals){
			Vector3 pos = PointOnCircle(center, radius, i);
			// make the object face the center
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center-pos);
			GameObject newObj = (GameObject) Instantiate(prefab, pos, rot);
			newObjs.Add (newObj);
			RVOMove newRVOMove = newObj.GetComponent<RVOMove> ();
			GameObject newGoal = new GameObject ();
			newGoal.transform.position = pos + newObj.transform.forward * radius * 2;
			newRVOMove.goal = newGoal.transform;
//			Vector3 diff = newRVOMove.goal.position - newObj.transform.position;
//			float dist = diff.magnitude;
//			//newRVOMove.max_speed = ((diff / dist) * 2).magnitude;
			newObj.SetActive (false);
		}

		foreach (GameObject o in newObjs) {
			o.GetComponent<RVOMove>().maxNeighbors = numObjects / Mathf.PI;
			o.GetComponent<RVOMove>().neighborDist = radius * 2;
			o.SetActive (true);
		}

	}

	void Update () {
		
	}
}
