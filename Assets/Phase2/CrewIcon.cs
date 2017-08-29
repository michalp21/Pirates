using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//Use delegate/event to call back to crewmanager class.
public class CrewIcon : MonoBehaviour, IPointerUpHandler { //Don't inherit from MonoBehaviour
	public int number { get; private set; } //number of crewmembers associated with this icon
	public string name { get; private set; }
	public GameObject crewPrefab { get; private set; } //prefab to instantiate when this icon is selected and user clicks on ground
	CrewManager myManager; //the class which handles array of CrewIcons
	Ray ray;
	RaycastHit hit;

	//credit: Cawas
	//Factory method, behaving similar to AddComponent, but with ability to add parameters
	public static CrewIcon CreateComponent (GameObject go, int i, string n, GameObject p, CrewManager cm) {
		CrewIcon myC = go.AddComponent<CrewIcon>();
		myC.number = i;
		myC.name = n;
		myC.crewPrefab = p;
		myC.myManager = cm;
		return myC;
	}

	//** Use Reference to CrewManager for selecting CrewIcon **
	//Detect when an icon is clicked (to select troops to deploy)
	//Uses reference to CrewManger
	public void OnPointerUp(PointerEventData eventData)
	{
		myManager.changeSelected (name);
	}

	public void InstantiateCrew() {
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			Instantiate (crewPrefab, new Vector3 (hit.point.x, hit.point.y, hit.point.z), Quaternion.identity);
		}
		number--;
	}

	void Update() {
		
	}
}
