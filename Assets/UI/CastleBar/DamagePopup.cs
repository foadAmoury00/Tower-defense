using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    
    public void SetDamageText(float damage)
    {
        if (damageText != null)
        {
            damageText.text = "-" + damage.ToString("F0");
        }
    }
    
    public void SetTextColor(Color color)
    {
        if (damageText != null)
        {
            damageText.color = color;
        }
    }
}