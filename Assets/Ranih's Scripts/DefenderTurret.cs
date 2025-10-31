using UnityEngine;

[RequireComponent(typeof(AutoAttack))]
public class DefenderTurret : MonoBehaviour
{
    [Header("Combat")]
    public float attackRange = 10f;
    public float timeBetweenShots = 0.75f;
    public float projectileDamage = 10f;

    private AutoAttack aa;

    private void Awake()
    {
        aa = GetComponent<AutoAttack>();
        // Push values into the AutoAttack at runtime so one prefab can adapt per recruitment
        aa.attackRange = attackRange;
        aa.timeBetweenShots = timeBetweenShots;
        aa.projectileDamage = projectileDamage;
        // Make sure this turret never moves:
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    private void OnDestroy()
    {
        if (RecruitManager.Instance != null)
            RecruitManager.Instance.OnDefenderDestroyed();
    }
}
