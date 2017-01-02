using UnityEngine;
using System.Collections;

public abstract class BaseShip : MonoBehaviour {
	
	public int WeaponSpace_x { get; set; }
	public int WeaponSpace_y { get; set; }
	public int PeopleSpace { get; set; }
	public string ShipName { get; set; }
	public int Rarity { get; set; }
	
	public abstract void fireMainWeapon();
}
