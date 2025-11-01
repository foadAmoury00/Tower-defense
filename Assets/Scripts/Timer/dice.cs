using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class Dice : MonoBehaviour
{
    public Animator animator;
    public TMP_Text resultText;
    public float addedTime = 15f;

    private bool isRolling = false;

    public Timer timer;

    int noRepeating;

    
    public Action DiceCompleted;
   

    void Start()
    {
        noRepeating = -1;
        resultText.text = "Press R to roll the dice!";
        //animator.Play("Idle");

        animator = GetComponent<Animator>();    
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        RollDice();
    //    }
    //}

    public void RollDice()
    {
        if (isRolling) return;
        StartCoroutine(RollCoroutine());
    }

    private IEnumerator RollCoroutine()
    {
        isRolling = true;
        resultText.text = "Rolling...";

        int result = UnityEngine.Random.Range(1, 7);
        animator.Play(result.ToString());

        yield return new WaitForSeconds(GetCurrentAnimationLength());

        DiceCompleted?.Invoke();

        resultText.text = "You rolled: " + result;

        if (addedTime > 0)
        {
            
            if (timer != null)
                timer.AddTime(addedTime);
        }

        isRolling = false;
    }

    private float GetCurrentAnimationLength()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        return state.length;
    }
}
