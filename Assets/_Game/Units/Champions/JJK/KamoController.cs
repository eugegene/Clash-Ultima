using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitAttack))]
public class KamoController : MonoBehaviour
{
    [Header("Q: Blood Arrows Settings")]
    public float healthCost = 10f;
    public float energyCost = 5f; 
    public float bonusDamage = 15f;
    
    [Tooltip("Speed of the Blood Arrow projectile")]
    public float bloodArrowSpeed = 35f;

    public bool isBloodArrowsActive = false;

    [Header("Visuals")]
    public Color bloodColor = Color.red;

    // --- NEW: PASSIVE THRESHOLD ---
    private float _bloodThresholdPercent = 0f;

    private UnitStats _stats;
    private UnitAttack _attack;

    void Awake()
    {
        _stats = GetComponent<UnitStats>();
        _attack = GetComponent<UnitAttack>();
    }

    void OnEnable()
    {
        if (_attack != null)
            _attack.OnProjectileLaunched += ModifyArrow;
    }

    void OnDisable()
    {
        if (_attack != null)
            _attack.OnProjectileLaunched -= ModifyArrow;
    }

    // Called by Passive_KamoClan.OnEquip
    public void SetBloodThreshold(float percent)
    {
        _bloodThresholdPercent = percent;
    }

    // Centralized Helper: Use this for ALL blood abilities
    public bool CanUseBloodTechnique()
    {
        float threshold = _stats.MaxHealth.Value * _bloodThresholdPercent;
        
        // Strict check: Must be ABOVE 30%, not AT 30%
        return _stats.CurrentHealth > threshold;
    }

    public void ToggleBloodArrows()
    {
        isBloodArrowsActive = !isBloodArrowsActive;
    }

    private void ModifyArrow(SimpleProjectile projectile)
    {
        if (!isBloodArrowsActive || projectile == null) return;

        // 1. Check Passive Restriction FIRST
        if (!CanUseBloodTechnique())
        {
            isBloodArrowsActive = false; // Auto-toggle off
            Debug.Log("<color=red>HP too low (Risk Zone)! Blood Technique disabled.</color>");
            return;
        }

        // 2. Check Costs
        if (_stats.CurrentHealth >= healthCost &&
            _stats.CurrentResource >= energyCost)
        {
            _stats.ModifyHealth(-healthCost);
            _stats.ModifyResource(-energyCost);

            if (_attack.currentTarget != null)
                projectile.SetHomingTarget(_attack.currentTarget);

            projectile.SetDamage(projectile.BaseDamage + bonusDamage);
            projectile.SetSpeed(bloodArrowSpeed);

            ApplyBloodVisuals(projectile);
        }
        else
        {
            isBloodArrowsActive = false;
            Debug.Log("Out of Blood or Cursed Energy!");
        }
    }

    private void ApplyBloodVisuals(SimpleProjectile projectile)
    {
        Renderer r = projectile.GetComponentInChildren<Renderer>();
        if (r != null) r.material.color = bloodColor;

        TrailRenderer t = projectile.GetComponentInChildren<TrailRenderer>();
        if (t != null) t.startColor = bloodColor;
    }
}