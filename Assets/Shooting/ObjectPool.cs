using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
///  This was a basic object pool found somewhere in the forums... forget where I got it so I can't give proper credit and for that I am sorry.
///  It has been altered in all areas so I guess it isn't the same as I got  on the forums anyway but the basis is the same.
///  
///  You set the GameObjects you want to be pooled in the objectPrefabs array
///  You can set the amount to start each pool size in the  amountToBuffer array (make sure both have the same index)
///  
///   On Start it will create the objects and  disable them so they are ready to be pooled
///   I added a  Send Message of "ResetMe" to each object grabbed out of the pool, I use this to set pooled objects
///       to their default settings ( like enemy health, otherwise you run into interesting results like grabbing dead enemies)
/// </summary>


public class ObjectPool : MonoBehaviour
{
    public static ObjectPool pool;  // a static handler to use the pool from anywhere in the  project once it is created

    /// <summary>
    /// The object prefabs which the pool can handle.
    /// </summary>
    public GameObject[] objectPrefabs;

    /// <summary>
    /// The pooled objects currently available.
    /// </summary>
    public List<GameObject>[] pooledObjects;

    /// <summary>
    /// The amount of objects of each type to buffer.
    /// </summary>
    public int[] amountToBuffer;

    public int defaultBufferAmount = 3;

    /// <summary>
    /// The container object that we will keep unused pooled objects so we dont clog up the editor with objects.
    /// </summary>
    protected GameObject containerObject;

    void Awake()
    {
        pool = this;
    }

    // Use this for initialization
    void Start()
    {
        containerObject = new GameObject("ObjectPool"); // I set the container to the pool itself, change it to whatever here

        //Loop through the object prefabs and make a new list for each one.
        //We do this because the pool can only support prefabs set to it in the editor,
        //so we can assume the lists of pooled objects are in the same order as object prefabs in the array
        pooledObjects = new List<GameObject>[objectPrefabs.Length];

        int i = 0;
        foreach (GameObject objectPrefab in objectPrefabs)
        {
            pooledObjects[i] = new List<GameObject>();

            int bufferAmount;

            if (i < amountToBuffer.Length) bufferAmount = amountToBuffer[i];
            else
                bufferAmount = defaultBufferAmount;

            for (int n = 0; n < bufferAmount; n++)
            {
                GameObject newObj = Instantiate(objectPrefab) as GameObject;  // create an object
                newObj.name = objectPrefab.name; // I don't like Clone on my names so I change the name to the prefab name
                PoolObject(newObj); // pool the new object
            }

            i++;
        }
    }

    /// <summary>
    /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
    /// then null will be returned.
    /// </summary>
    /// <returns>
    /// The object for type.
    /// </returns>
    /// <param name='objectType'>
    /// Object type.
    /// </param>
    /// <param name='onlyPooled'>
    /// If true, it will only return an object if there is one currently pooled.
    /// </param>
    public GameObject GetObjectForType(string objectType, bool onlyPooled)
    {
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            GameObject prefab = objectPrefabs[i];

           // Debug.Log(" if ( "+prefab.name+" == "+ objectType+" )");
            if (prefab.name == objectType)
            {
                if (pooledObjects[i].Count > 0)
                {
                    GameObject pooledObject = pooledObjects[i][0];
                    pooledObjects[i].RemoveAt(0);
                    pooledObject.transform.parent = null;
                    pooledObject.SetActive(true);

                    //a few objects need to have some values reset... send them a message to do so, others will ignore it
                    pooledObject.SendMessageUpwards("ResetMe", SendMessageOptions.DontRequireReceiver);

                    return pooledObject;

                }
                else if (!onlyPooled)
                {
					//Debug.Log ("Created a new " + objectPrefabs[i].name + " beacuse not enough were pooled.");
					GameObject pooledObject = Instantiate(objectPrefabs[i]) as GameObject;
					pooledObject.transform.parent = null;
					pooledObject.SetActive(true);
					pooledObject.SendMessageUpwards("ResetMe", SendMessageOptions.DontRequireReceiver);
					pooledObject.name = objectPrefabs[i].name;
					return pooledObject;
                }

                break;
            }
        }

        //If we have gotten here either there was no object of the specified type or none were left in the pool with onlyPooled set to true
        return null;
    }

    /// <summary>
    /// Pools the object specified.  Will not be pooled if there is no prefab of that type.
    /// </summary>
    /// <param name='obj'>
    /// Object to be pooled.
    /// </param>
    public void PoolObject(GameObject obj)
    {
        // If onlyPooled was set to false, when you run out of pooled objects it will instantiate a new object
        // this appends (Clone) to the end of the name and it won't pool properly... instead of destroying the new object
        // change the name to fit the pooled type name so it gets added to the pool
        // this allows you to set a lower starting pool size and it will dynamicly increase the pool size when needed
        if (obj.name.Contains("(Clone)"))
        {
            // if the obj contains clone (must have been instantiated) remove it from the name so we can pool it below
            char[] rem = {'(', 'C', 'l', 'o', 'n', 'e', ')'};
            obj.name = obj.name.TrimEnd(rem);           
        }

        // this part actually does the pooling
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            if (objectPrefabs[i].name == obj.name)
            {
                obj.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f); // move the pooled object out of the game space
                obj.SetActive(false);
                obj.transform.parent = containerObject.transform;
                pooledObjects[i].Add(obj);
                return;
            }
        } 
    }
}
