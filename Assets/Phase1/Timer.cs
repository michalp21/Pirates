using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Credit: rober-psious
public class Timer : MonoBehaviour
{
    public int Minutes = 0;
    public int Seconds = 0;

    private Text m_text;
    private float m_leftTime;

    public delegate void UnitEventHandler();
    public static event UnitEventHandler onTimerZero;

    private void Awake()
    {
        m_text = GetComponent<Text>();
        m_leftTime = GetInitialTime();
    }

    private void Update()
    {
        if (m_leftTime > 0f)
        {
            //  Update countdown clock
            m_leftTime -= Time.deltaTime;
            Minutes = GetLeftMinutes();
            Seconds = GetLeftSeconds();

            //  Show current clock
            if (m_leftTime > 0f)
            {
                m_text.text = "Time : " + Minutes + ":" + Seconds.ToString("00");
            }
            else
            {
                //  The countdown clock has finished
                m_text.text = "Time : 0:00";
            }
        }
        else
        {
            if (onTimerZero != null)
                onTimerZero();
        }
    }

    private float GetInitialTime()
    {
        return Minutes * 60f + Seconds;
    }

    private int GetLeftMinutes()
    {
        return Mathf.FloorToInt(m_leftTime / 60f);
    }

    private int GetLeftSeconds()
    {
        return Mathf.FloorToInt(m_leftTime % 60f);
    }
}