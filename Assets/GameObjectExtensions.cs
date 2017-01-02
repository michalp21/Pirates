using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

//May not use this
public class GameObjectExtensions
{
	private static void InvokeParameterisedAwake(GameObject newObject, object[] arg)
	{
		foreach (MonoBehaviour behaviour in newObject.GetComponents<MonoBehaviour>())
		{
			foreach (MethodInfo info in behaviour.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))// | BindingFlags.DeclaredOnly))
			{
				if (String.Compare(info.Name, "Awake") == 0)
				{
					ParameterInfo[] parameters = info.GetParameters();
					if (parameters.Length == arg.Length)
					{
						bool typesMatch = true;
						for (int i = 0; i < (parameters.Length) && (typesMatch == true); i++)
						{
							if (parameters[i].ParameterType != arg[i].GetType())
							{
								typesMatch = false;
							}
						}
						
						if (typesMatch == true)
						{
							info.Invoke(behaviour, arg);
						}
						
					}
					
				}
			}
		}
	}
	public static GameObject Instantiate(GameObject O, params object[] arg)
	{
		GameObject newObject = GameObject.Instantiate(O) as GameObject;
		
		InvokeParameterisedAwake(newObject, arg);
		return newObject;
	}
	
}

public class Instantiate : MonoBehaviour
{
	// We must declare the standard Awake method otherwise Unity generates a compile error
	void Awake()
	{
	}
	
	// an Awake method with an integer parameter
	// NOTE: We have to declare it as public!
	public void Awake(int startUpValue)
	{
		Debug.Log("Parameterised awake with integer parameter invoked!");
		Debug.Log(String.Format("Start up value = {0}", startUpValue));
	}
	
	// an Awake method with a string parameter
	// NOTE: We have to declare it as public!
	public void Awake(string msg)
	{
		Debug.Log("Parameterised awake with string parameter invoked!");
		Debug.Log(String.Format("Message = {0}", msg));
	}
	
	// an Awake method with two float parameters and an integer
	// NOTE: We have to declare it as public!
	public void Awake(float a, float b, int c)
	{
		Debug.Log("Parameterised awake with three parameters invoked!");
		Debug.Log(String.Format("The values = {0:N}, {1:N}, {2}", a, b, c));
	}
	
}