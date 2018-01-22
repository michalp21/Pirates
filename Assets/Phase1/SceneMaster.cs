using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMaster : MonoBehaviour {

    void Phase1ToPhase2()
    {
        SceneManager.LoadScene("Phase2");
        Debug.Log("wakawakawaka");
    }

	// Use this for initialization
	void Start () {
        Timer.onTimerZero += Phase1ToPhase2;
	}

    void OnDisable()
    {
        Timer.onTimerZero -= Phase1ToPhase2;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
