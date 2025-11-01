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

    int noRepeating;

    void Start()
    {
        noRepeating = -1;
        resultText.text = "Press R to roll the dice!";
        diceAnimator.Play("Idle");
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
        resultText.text = "Rolling...";

        int result = Random.Range(1, 7);
        diceAnimator.Play(result.ToString());

        yield return new WaitForSeconds(GetCurrentAnimationLength());

        resultText.text = "You rolled: " + result;

        if (addedTime > 0)
        {
            
            if (timer != null)
                timer.AddTime(addedTime);
        }

        diceAnimator.Play("Idle");
        isRolling = false;
    }

    private float GetCurrentAnimationLength()
    {
        AnimatorStateInfo state = diceAnimator.GetCurrentAnimatorStateInfo(0);
        return state.length;
    }
}
