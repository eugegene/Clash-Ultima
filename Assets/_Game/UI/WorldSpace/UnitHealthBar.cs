using UnityEngine;
using UnityEngine.UI; // <--- VITAL: This lets us use "Image" and "Canvas"

public class UnitHealthBar : MonoBehaviour
{
    // --- VARIABLES SECTION (You were missing this!) ---
    [Header("Setup")]
    public UnitStats targetUnit;
    public Image fillImage;
    public Canvas canvas;

    private Camera _cam;
    // --------------------------------------------------

    void Start()
    {
        _cam = Camera.main;

        // AUTO-FIND LOGIC:
        // If we forgot to assign a target manually, look in the parent object
        if (targetUnit == null)
        {
            targetUnit = GetComponentInParent<UnitStats>();
        }

        // Optimization: Turn off if STILL no target found
        if (targetUnit == null) 
        {
            gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;

        if (targetUnit == null) return;

        // 1. Update Fill Amount based on HP
        if (fillImage != null)
        {
            float pct = targetUnit.CurrentHealth / targetUnit.MaxHealth.Value;
            fillImage.fillAmount = pct;
        }
    }
}