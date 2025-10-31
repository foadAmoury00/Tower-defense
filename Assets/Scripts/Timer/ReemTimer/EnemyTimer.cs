using UnityEngine;

public class EnemyDeathNotifier : MonoBehaviour
{
    void OnDestroy()
    {
        // ≈–« ﬂ«‰ Â–« ⁄œÊ° √—”· ÕœÀ «·„Ê  ·· «Ì„—
        if (gameObject.CompareTag("Enemy"))
        {
            NotifyTimer();
        }
    }

    void NotifyTimer()
    {
        // «·»ÕÀ ⁄‰ «· «Ì„— ›Ì «·„‘Âœ
        EnemyTimer timer = FindObjectOfType<EnemyTimer>();
        if (timer != null)
        {
            timer.AddTime(5f);
        }
    }
}