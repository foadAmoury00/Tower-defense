using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
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

        timerText.text = "Time: " + Mathf.Ceil(timeLeft);
    }

    public void AddTime(float seconds)
    {
        timeLeft += seconds;
    }
}