using UnityEngine;
using TMPro;
using System.Collections;

public class Dice : MonoBehaviour
{
    public Animator diceAnimator;
    public TMP_Text resultText;
    public float addedTime = 15f;

    private bool isRolling = false;
    public Timer timer;

    void Start()
    {
        resultText.text = "Press R to roll the dice!";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RollDice();
        }
    }

    public void RollDice()
    {
        if (isRolling) return;
        StartCoroutine(RollCoroutine());
    }

    private IEnumerator RollCoroutine()
    {
        isRolling = true;
        diceAnimator.speed = 1f;
        resultText.text = "Rolling...";

        int result = Random.Range(1, 7);
        diceAnimator.Play(result.ToString(), 0, 0f);

        yield return new WaitForSeconds(3f);
        diceAnimator.speed = 0f;

        resultText.text = "You rolled: " + result;

        if (addedTime > 0 && timer != null)
            timer.AddTime(addedTime);

        isRolling = false;
    }
}
