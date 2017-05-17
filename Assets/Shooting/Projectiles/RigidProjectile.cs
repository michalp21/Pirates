using UnityEngine;
using System.Collections;

public class RigidProjectile : Projectile
{    
    private Rigidbody myRigid;   

    public override void SetUp(ProjectileInfo info)
    {
        base.SetUp(info);
        myRigid = GetComponent<Rigidbody>();       
		myRigid.velocity = velocity;
    }

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Water") &&
		   (myInfo.psurvive == ProjectileSurvive.AIR))
		{// if we hit water... kill the bullet IF it won't survive underwater (not fully implemented yet)
			CancelInvoke("Recycle");
			Recycle();
		}

		//do one for air too

		else if (other.gameObject.tag.Contains(myInfo.enemyID) &&
				 other.gameObject.tag.Contains("Weapon"))
		{
			//coll.GetComponent<healthScript>().health -= 1;

			Health hitObject = other.gameObject.GetComponent<Health>();
			hitObject.Hit(myInfo); // send bullet info to hit object's health component
			
			hitCount++; // add a hit
			
			if (hitCount > myInfo.maxPenetration)
			{
				CancelInvoke("Recycle");
				Recycle(); // if hit count exceeds max hits.... kill the bullet
			}
		}
	}

	//UPDATE THIS TO WORK FOR TRIGGERS, not RAYCASTS (changed to OnTriggerEnter)
//    void FixedUpdate()
//    {
//        RaycastHit hit;  // forward hit
//        RaycastHit hit2; // rear hit       
//        
//        if ()
//        {
//            // probably shouldn't do this but best way i can think of to avoid
//            // multiple hits from same bullet
//            myRigid.MovePosition(hit.point); // move the bullet to the impact point
//            transform.position = hit.point;
//            
//            if (hit.transform.CompareTag("Water"))
//            {// if we hit water... kill the bullet IF it won't survive underwater (not fully implemented yet)
//                CancelInvoke("Recycle");
//                Recycle();
//            }
//
//            Health hitObject = hit.transform.GetComponent<Health>();
//
//            if (hitObject)
//            {                
//                hitObject.Hit(myInfo); // send bullet info to hit object's health component
//            }
//			else
//            {
//                //MakeAHole(hit); // make a hole anywhere except the players
//            }
//	
//            hitCount++; // add a hit
//
//            if (hitCount > myInfo.maxPenetration)
//            {
//                CancelInvoke("Recycle");
//                Recycle(); // if hit count exceeds max hits.... kill the bullet
//            }
//        }
//    }   
}
