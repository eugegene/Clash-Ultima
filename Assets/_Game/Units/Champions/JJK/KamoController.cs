// KamoController.cs
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitAttack))]
public class KamoController : MonoBehaviour
{
    [Header("Q: Blood Arrows Settings")]
    public float healthCost = 10f;
    public float energyCost = 5f; // Cost per arrow
    public float bonusDamage = 15f;

    public bool isBloodArrowsActive = false;

    [Header("Visuals")]
    public Color bloodColor = Color.red;

    private UnitStats _stats;
    private UnitAttack _attack;

    void Awake()
    {
        _stats = GetComponent<UnitStats>();
        _attack = GetComponent<UnitAttack>();
    }

    void OnEnable()
    {
        if (_attack != null) _attack.OnProjectileLaunched += ModifyArrow;
    }

    void OnDisable()
    {
        if (_attack != null) _attack.OnProjectileLaunched -= ModifyArrow;
    }

    public void ToggleBloodArrows()
    {
        isBloodArrowsActive = !isBloodArrowsActive;
    }

    private void ModifyArrow(SimpleProjectile projectile)
    {
        if (!isBloodArrowsActive || projectile == null) return;

        // Validate resources
        if (_stats.CurrentHealth >= healthCost && _stats.CurrentResource >= energyCost)
        {
            // Pay costs
            _stats.ModifyHealth(-healthCost);
            _stats.ModifyResource(-energyCost);

            // Make homing if we have a valid attack target
            if (_attack.currentTarget != null)
            {
                projectile.SetHomingTarget(_attack.currentTarget);
            }

            // Apply bonus damage on top of base damage (preserve crit flag)
            float baseDmg = projectile.BaseDamage;
            float newDamage = baseDmg + bonusDamage;
            projectile.SetDamage(newDamage);

            // Visuals: change material instance color + trail
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
        if (projectile == null) return;

        Renderer rend = projectile.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            // Accessing .material creates an instance so shared material is not modified
            rend.material.color = bloodColor;
        }

        TrailRenderer trail = projectile.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
        {

            trail.startColor = bloodColor;
        }
    }
}
