using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Won't use
public class Column : MonoBehaviour {
	public int gunsRemaining { get; set; }
	private Gun[] guns;

	public Gun this[int row]
	{
		get
		{
			if (row >= 0 && row < guns.Length)
				return guns[row];
			throw new System.IndexOutOfRangeException();
		}
		set
		{
			if (row >= 0 && row < guns.Length)
				guns [row] = value;
			throw new System.IndexOutOfRangeException();
		}
	}

	public int GetLen() {
		return guns.Length;
	}

	public void RemoveWeaponFromColumn(int row) {
		guns [row] = null;
		gunsRemaining--;
		if (gunsRemaining == 0) {
			KillColumn ();
		}
	}

	public void KillColumn() {
		for(int i = 0; i < guns.Length; i++) {
			if (guns [i] != null) {
				guns [i].GetComponent<Health> ().Die ();
				guns [i] = null;
				gunsRemaining--;
			}
		}
		Rigidbody myRigidbody = GetComponent<Rigidbody> ();
		myRigidbody.useGravity = true;
		Vector3 dir = Quaternion.AngleAxis(60, Vector3.up) * Vector3.right;
		myRigidbody.AddForce(dir*2);
	}

	public void initColumn() {
		guns = GetComponentsInChildren<Gun> ();
		gunsRemaining = guns.Length;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
