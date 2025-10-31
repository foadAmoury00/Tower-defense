using UnityEngine;

public class EnemyDeathNotifier : MonoBehaviour
{
    void OnDestroy()
    {
        // ��� ��� ��� ��� ���� ��� ����� �������
        if (gameObject.CompareTag("Enemy"))
        {
            NotifyTimer();
        }
    }

    void NotifyTimer()
    {
        // ����� �� ������� �� ������
        EnemyTimer timer = FindObjectOfType<EnemyTimer>();
        if (timer != null)
        {
            timer.AddTime(5f);
        }
    }
}