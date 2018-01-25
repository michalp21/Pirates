using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Scene Manager Interface?
//consider moving all scene transitions into a public static SceneTransitions class

public class Phase2Master : MonoBehaviour {
    Timer timer;
    GameObject goal;
    bool reachedEnd = false;

    void CheckWinner()
    {
        
    }

    // Use this for initialization
    void Start()
    {
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        goal = GameObject.FindGameObjectWithTag("Goal");
    }
    
    void Update () {
        //if (!reachedEnd)
        //{
        //    if (timer.LeftTime <= 0)
        //    {
        //        reachedEnd = true;

        //        CheckWinner();
        //        StartCoroutine(AdvanceScene());

        //    }
        //    else if (goal.currentHealth <= 0 || goal.currentHealth <= 0)
        //    {
        //        reachedEnd = true;

        //        CheckWinner();
        //        StartCoroutine(AdvanceScene());
        //    }

        //}
    }

    IEnumerator AdvanceScene()
    {
        yield return new WaitForSeconds(3f);
        //SceneManager.LoadScene("Home");
    }
}
