using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    public Timer_UI timerUI;
    public TMP_Text timerText;
    public float timeLeft = 60f;
    private bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            timeLeft = 0;
            isRunning = false;
        }

        timerUI.UpdateTimer(Mathf.Ceil(timeLeft), 60f);
    }



    public void AddTime(float seconds)
    {
        timeLeft += seconds;
        timerUI.ExtendTime(Mathf.Ceil(timeLeft), 60f);
    }
}

