using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    public static RecruitManager Instance { get; private set; }

    [Header("Recruitment")]
    public int maxAllies = 3;
    private int currentAllies = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Reserve a slot for converting in-place
    public bool TryReserveSlot()
    {
        if (currentAllies >= maxAllies) return false;
        currentAllies++;
        return true;
    }

    public void ReleaseSlot()
    {
        currentAllies = Mathf.Max(0, currentAllies - 1);
    }

    public void IncreaseCap(int amount)
    {
        maxAllies = Mathf.Max(0, maxAllies + amount);
    }
}
