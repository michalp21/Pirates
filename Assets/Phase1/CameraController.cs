using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	// Reference to the player
	public GameObject player;
	
	// Offset from player to the camera
	private Vector3 offset;
	
	// Use this for initialization
	void Start () {
		// Offset is the camera pos - player pos
		offset = this.transform.position - player.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		// Update camera pos by adding the offset to player pos
		this.transform.position = player.transform.position + offset;
	}
}
