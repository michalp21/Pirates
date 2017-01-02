using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// Just a script to attach bullet hole prefabs to so the weapon/projectile scripts can
/// access them easily and can be changed out without changing weapon/projectile code
/// 
/// </summary>
/// 

public enum MaterialTypes
{
    UNKNOWN,
    WOOD,
    STONE,
    METAL
}


public class BulletHoleManager : MonoBehaviour
{
    public static BulletHoleManager bulletHole;
    
    public List<GameObject> holes = new List<GameObject>(); // generic holes
    public List<GameObject> woodHoles = new List<GameObject>();
    public List<GameObject> metalHoles = new List<GameObject>();
    public List<GameObject> stoneHoles = new List<GameObject>();

    public List<GameObject> activeHoles = new List<GameObject>(); // currently active bullet holes


    void Start()
    {
        bulletHole = this;
    }


    public void SpawnHole(RaycastHit hit)
    {
        Vector3 pos = hit.point + hit.normal * 0.01f;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
        int i = 0;
        GameObject newHole;        
        MaterialTypes mat = MaterialTypes.UNKNOWN;

        switch (hit.transform.tag)
        {
            case "Wood":
                mat = MaterialTypes.WOOD;
                break;
            default:
                mat = MaterialTypes.UNKNOWN;
                break;
        }

        
        switch (mat)
        {
            case MaterialTypes.METAL:
                i = Random.Range(0, metalHoles.Count);
                newHole = ObjectPool.pool.GetObjectForType(metalHoles[i].name, false);
                newHole.transform.position = pos;
                newHole.transform.rotation = rot;
                break;
            case MaterialTypes.STONE:
                i = Random.Range(0, stoneHoles.Count);
                newHole = ObjectPool.pool.GetObjectForType(stoneHoles[i].name, false);
                newHole.transform.position = pos;
                newHole.transform.rotation = rot;
                break;
            case MaterialTypes.WOOD:
                i = Random.Range(0, woodHoles.Count);
                newHole = ObjectPool.pool.GetObjectForType(woodHoles[i].name, false);
                newHole.transform.position = pos;
                newHole.transform.rotation = rot;
                break;
            default:
                i = Random.Range(0, holes.Count);
                newHole = ObjectPool.pool.GetObjectForType(holes[i].name, false);
                newHole.transform.position = pos;
                newHole.transform.rotation = rot;
                break;
        }

        newHole.GetComponent<BulletHole>().Set();
        newHole.transform.parent = transform;
        activeHoles.Add(newHole);
    }
}


