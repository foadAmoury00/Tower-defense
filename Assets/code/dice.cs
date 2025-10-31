using UnityEngine;
using TMPro;
using System.Collections;

public class dice : MonoBehaviour
{
    public Transform diceTransform;
    public TMP_Text resultText;
    public float addedTime = 15f;

    private bool isRolling = false;

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
        resultText.text = "Rolling...";

        Quaternion startRot = diceTransform.rotation;
        Quaternion randomRot = Random.rotation;

        float elapsed = 0f;
        float duration = 3f;

        while (elapsed < duration)
        {
            diceTransform.rotation = Quaternion.Slerp(startRot, randomRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        diceTransform.rotation = randomRot;

        int result = DetectNumber();
        resultText.text = "You rolled: " + result;

        if (addedTime > 0)
        {
            Timer timer = FindObjectOfType<Timer>();
            if (timer != null)
                timer.AddTime(addedTime);
        }

        isRolling = false;
    }

    int DetectNumber()
    {
        Vector3 up = diceTransform.up;
        float x = Vector3.Dot(Vector3.up, diceTransform.right);
        float y = Vector3.Dot(Vector3.up, diceTransform.up);
        float z = Vector3.Dot(Vector3.up, diceTransform.forward);

        if (Mathf.Abs(x) > Mathf.Abs(y) && Mathf.Abs(x) > Mathf.Abs(z))
            return x > 0 ? 3 : 4;
        else if (Mathf.Abs(y) > Mathf.Abs(x) && Mathf.Abs(y) > Mathf.Abs(z))
            return y > 0 ? 1 : 6;
        else
            return z > 0 ? 2 : 5;
    }
}
