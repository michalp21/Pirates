using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Scene Manager Interface?
//consider moving all scene transitions into a public static SceneTransitions class

public class Phase1Master : MonoBehaviour {
    public float EPSILON = .001f;
    Timer timer;
    WeaponManager wmSelf;   //0
    WeaponManager wmOther;  //1
    bool reachedEnd = false;

    void Phase1ToPhase2()
    {
        SceneManager.LoadScene("Phase2");
        Debug.Log("wakawakawaka");
    }

    void CheckWinner()
    {
        if (wmSelf.currentHealth == wmOther.currentHealth)
            PersistentData.phase1_winner = -1;
        else if (wmSelf.currentHealth < wmOther.currentHealth)
            PersistentData.phase1_winner = 1;
        else if (wmSelf.currentHealth > wmOther.currentHealth)
            PersistentData.phase1_winner = 0;
    }

    // Use this for initialization
    void Start()
    {
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        wmSelf = GameObject.FindGameObjectWithTag("ship_self").GetComponent<WeaponManager>();
        wmOther = GameObject.FindGameObjectWithTag("ship_other").GetComponent<WeaponManager>();
    }
	
	void Update () {
        //Timer runs out -> ships ram -> subtract min(ship1health, ship2health) from ship1health and ship2health
        if (!reachedEnd)
        {
            if (timer.LeftTime <= 0)
            {
                reachedEnd = true;

                wmSelf.isFiring = false;
                wmOther.isFiring = false;
                wmSelf.MoveShip(1);
                wmOther.MoveShip(-1);

                int remainingHealth = Mathf.Abs(wmSelf.currentHealth - wmOther.currentHealth);
                wmSelf.TakeDamage(remainingHealth);
                wmOther.TakeDamage(remainingHealth);

                float loadNextScene = Time.time + 2;
                if (loadNextScene < Time.time)
                    CheckWinner();
                    StartCoroutine(AdvanceScene());

            }
            else if (wmSelf.currentHealth <= 0 || wmOther.currentHealth <= 0)
            {
                reachedEnd = true;

                wmSelf.isFiring = false;
                wmOther.isFiring = false;
                CheckWinner();
                StartCoroutine(AdvanceScene());
            }

            Debug.Log(wmOther.currentHealth);
        }
	}

    IEnumerator AdvanceScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Phase2");
    }
}
