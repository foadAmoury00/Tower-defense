using UnityEngine;
using UnityEngine.UI;

public class EnemyTimer : MonoBehaviour
{
    public float currentTime = 60f;
    public Text timerText;

    void Start()
    {
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                currentTime = 0;
                UpdateTimerDisplay();
                Debug.Log("⏰ TIME UP!");
            }
        }
    }

    public void AddTime(float seconds)
    {
        currentTime += seconds;
        UpdateTimerDisplay();
        Debug.Log("➕ +" + seconds + " seconds!");
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.RoundToInt(currentTime).ToString() + "s";
        }
    }
}