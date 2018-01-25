using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

//Use delegate/event to call back to crewmanager class.
public class CrewIcon : MonoBehaviour, IPointerUpHandler { //Don't inherit from MonoBehaviour
	public int number { get; private set; } //number of crewmembers associated with this icon
	public string crewName { get; private set; }
	public GameObject crewPrefab { get; private set; } //prefab to instantiate when this icon is selected and user clicks on ground
	CrewManager myManager; //the class which handles array of CrewIcons
	Ray ray;
	RaycastHit hit;

	public const int LAYER_SELFSHOOTER = 11;
	public const int LAYER_OTHERSHOOTER = 12;
	public const int LAYER_GROUND = 13;
	public const int LAYER_OBSTACLES = 14;
	public const float COLLIDER_RADIUS = .4f; //accounts for scaling of transform

	//credit: Cawas
	//Factory method, behaving similar to AddComponent, but with ability to add parameters
	public static CrewIcon CreateComponent (GameObject go, int i, string n, GameObject p, CrewManager cm) {
		CrewIcon myC = go.AddComponent<CrewIcon>();
		myC.number = i;
		myC.crewName = n;
		myC.crewPrefab = p;
		myC.myManager = cm;
		return myC;
	}

	//** Use Reference to CrewManager for selecting CrewIcon **
	//Detect when an icon is clicked (to select troops to deploy)
	//Uses reference to CrewManger
	public void OnPointerUp(PointerEventData eventData)
	{
		myManager.changeSelected (crewName);
	}

	//target = closestObject;

	public void TryInstantiateCrew() {
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			if (hit.collider.gameObject.layer == LAYER_GROUND || hit.collider.gameObject.layer == LAYER_OTHERSHOOTER || hit.collider.gameObject.layer == LAYER_SELFSHOOTER) {
				InstantiateCrew ();
			}
		}
	}

	protected void InstantiateCrew(int depth = 0) {
		//Put layer filter for SELF_SHOOTER?

		/* (Assume we use CapsuleCollider)
		 * First use OverlapSphere to quickly grab nearby colliders, then remove non-enemy colliders
		 * Note: use radius of collider (should be a constant) for radius of overlap sphere
		 * If nearby enemy colliders are found, find the closest one
		 * If the distance between closest collider and raycast point is less than 2.1x radius of collider (2 colliders plus wiggle room),
		 * 	move instantiation position away from the raycast point, on a line normal to the nearest collider.
		 * If new instantiation position is too close to another collider or outside walkable territory, do not instantiate. 
		 * 
		 * WARNING: still a bit buggy but satisfactory for now. Maybe will replace with a better instantiation system, so don't want to waste time here.
		 * */

		Collider[] colliders = Physics.OverlapSphere (hit.point, COLLIDER_RADIUS);
		List<Collider> hitColliders = colliders.Cast<Collider>().ToList();
		hitColliders.RemoveAll(x => x.gameObject.layer != LAYER_OTHERSHOOTER && x.gameObject.layer != LAYER_OBSTACLES); //MAYBE CHANGE TO INCLUDE LAYER_SELFSHOOTER

		if (hitColliders.Count != 0) {
			float cutoff = COLLIDER_RADIUS * 2.1f;
			Collider nearest = hitColliders.OrderByDescending (c => (new Vector3 (hit.point.x, 0, hit.point.z) - new Vector3 (c.transform.position.x, 0, c.transform.position.z)).sqrMagnitude)
				.FirstOrDefault ();
			Vector3 dir = new Vector3 (hit.point.x, 0, hit.point.z) - new Vector3 (nearest.transform.position.x, 0, nearest.transform.position.z);
			//Debug.Log (dir.magnitude + " ~ " + cutoff + " :: " + nearest.name);

			if (dir.sqrMagnitude < cutoff * cutoff) {
				if (depth == 0) {
					float ratio = (float)((cutoff) / dir.magnitude);
					Vector3 newPoint = nearest.transform.position + dir.normalized * dir.magnitude * ratio;

					if (Physics.Raycast (newPoint, new Vector3 (0, -1, 0), out hit, 1)) {
						if (hit.collider.gameObject.layer == LAYER_GROUND) {
							InstantiateCrew (1);
						}
					}
				} else {
					Debug.Log ("CAN'T INSTANTIATE");
				}
			}
		} else {
			Vector3 newPoint = new Vector3 (hit.point.x, 0, hit.point.z);
			InstantiateCrewHelper (newPoint);
		}
		number--;
	}

	protected void InstantiateCrewHelper(Vector3 newPoint) {
		GameObject instantiatedCrew = Instantiate (crewPrefab, newPoint, Quaternion.identity);
		instantiatedCrew.layer = LAYER_SELFSHOOTER;
	}

	void Update() {
		
	}
}
