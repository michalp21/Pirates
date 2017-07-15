using UnityEngine;
using System.Collections;

//PROBABLY NOT USED
public interface IAimable {
	//select weapon -> select nearest/furthest/manual
	bool IsSelected { get; set; }
	Gun Target { get; set; }

	//default aiming
	void autoAim ();
	void targetNearest ();
	void targetFarthest ();
	void targetManual ();
	

}
