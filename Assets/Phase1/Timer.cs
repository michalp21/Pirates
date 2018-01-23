using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Credit: rober-psious
public class Timer : MonoBehaviour
{
    public int Minutes = 0;
    public int Seconds = 0;

    private Text leftTimeText;
    public float LeftTime { get; set; }

    private void Awake()
    {
        leftTimeText = GetComponent<Text>();
        LeftTime = GetInitialTime();
    }

    private void Update()
    {
        if (LeftTime > 0f)
        {
            //  Update countdown clock
            LeftTime -= Time.deltaTime;
            Minutes = GetLeftMinutes();
            Seconds = GetLeftSeconds();

            //  Show current clock
            if (LeftTime > 0f)
            {
                leftTimeText.text = "Time : " + Minutes + ":" + Seconds.ToString("00");
            }
            else
            {
                //  The countdown clock has finished
                leftTimeText.text = "Time : 0:00";
            }
        }
    }

    private float GetInitialTime()
    {
        return Minutes * 60f + Seconds;
    }

    private int GetLeftMinutes()
    {
        return Mathf.FloorToInt(LeftTime / 60f);
    }

    private int GetLeftSeconds()
    {
        return Mathf.FloorToInt(LeftTime % 60f);
    }
}