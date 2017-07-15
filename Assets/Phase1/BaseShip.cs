using UnityEngine;
using System.Collections;

public abstract class BaseShip : MonoBehaviour {
	
	public int weaponSpace_cols { get; set; }
	public int weaponSpace_rows { get; set; }
	public int peopleSpace { get; set; }
	public string shipName { get; set; }
	public int rarity { get; set; }
	
	public abstract void fireMainWeapon();
}
