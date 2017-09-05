using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct FetchedCrew {
	public int level;
	public string name;

	public FetchedCrew(int l, string n) {
		level = l;
		name = n;
	}
}

//Handles list of crewIcons
public class CrewManager : MonoBehaviour {
	GameObject[] possibleCrewPrefabs; //array of all possible prefabs
	List<FetchedCrew> currentCrew; //List of prefabs of current crew (after fetch)
	List<CrewIcon> crewIcons;
	int selected; //index of crewIcons
	[SerializeField] GameObject buttonPrefab;

	const int BUTTON_WIDTH = 200;
	const int BUTTON_SPACING = 7;
	const int LEFT_PADDING = 14;

	//Delegate for when something is spawned
	public delegate void UnitEventHandler ();
	public static event UnitEventHandler onInstantiated;

	// Probably get from some database later
	// Instantiate crew from prefab.
	void Start () {
		//Load all possible crew prefabs
		possibleCrewPrefabs = Resources.LoadAll("PossibleCrew", typeof(GameObject)).Cast<GameObject>().ToArray();
		//Find correct prefab from string
		fetchCrew ();
		//Init list of crew icons
		initCrewIcons ();

		selected = 0;
	}

	void OnDisable() {
		
	}

	//Fetch crew from database or something
	void fetchCrew() {
		currentCrew = new List<FetchedCrew>();
		//temp
		FetchedCrew aCrew = new FetchedCrew (1, "Reptile"); //DONT REPLACE FETCHEDCREW WITH CREW: because fetching from db will make things different
		currentCrew.Add (aCrew);
		currentCrew.Add (aCrew);
		Debug.Log (currentCrew[0] + " " + currentCrew[1]);
	}

	//Possibly refactor the "name" variables because it was hiding the gamobject "name"
	void initCrewIcons() {
		crewIcons = new List<CrewIcon> ();

		for (int i = 0; i < currentCrew.Count; i++) {
			CrewIcon aCrewIcon = null;
			for (int j = 0; j < possibleCrewPrefabs.Length; j++) {
				if (possibleCrewPrefabs [j].name == currentCrew [i].name) {
					//create button, add CrewIcon component, and set parent to (this) CrewManager
					GameObject myButton = (GameObject)Instantiate (buttonPrefab);
					aCrewIcon = CrewIcon.CreateComponent(myButton, 5, currentCrew [i].name, possibleCrewPrefabs [j], this); //add to newly created button
					RectTransform myRectTransform = myButton.GetComponent<RectTransform>();
					myRectTransform.localPosition += new Vector3(LEFT_PADDING + (BUTTON_WIDTH + BUTTON_SPACING) * crewIcons.Count,0,0);
					myButton.transform.SetParent(transform, false);
				}
			}
			//Keep track of icon in array
			if (aCrewIcon != null) {
				crewIcons.Add (aCrewIcon);
			}
		}
	}

	public void changeSelected(string s) {
		selected = crewIcons.FindIndex(a => a.crewName == s);
	}

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			crewIcons [selected].TryInstantiateCrew ();
			if (onInstantiated != null)
				onInstantiated ();
		}
	}
}
