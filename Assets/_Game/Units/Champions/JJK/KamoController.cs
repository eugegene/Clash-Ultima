using UnityEngine;

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
        // Debug.Log($"Blood Arrows: {isBloodArrowsActive}");
    }

    private void ModifyArrow(SimpleProjectile projectile)
    {
        if (!isBloodArrowsActive) return;

        // CHECK: Do we have enough HP AND Energy?
        if (_stats.CurrentHealth > healthCost && _stats.CurrentResource >= energyCost)
        {
            // 1. Pay Costs (HP and Cursed Energy)
            _stats.ModifyHealth(-healthCost);
            _stats.ModifyResource(-energyCost);

            // 2. Gameplay: Make Homing
            if (_attack.currentTarget != null)
            {
                projectile.SetHomingTarget(_attack.currentTarget);
            }

            // 3. Gameplay: Apply Bonus Damage
            // Calculate Base Damage + Bonus
            float newDamage = _stats.AttackDamage.Value + bonusDamage;
            projectile.SetDamage(newDamage);

            // 4. Visuals
            ApplyBloodVisuals(projectile);
        }
        else
        {
            // Turn off automatically if out of resources
            isBloodArrowsActive = false;
            Debug.Log("Out of Blood or Cursed Energy!");
        }
    }

    private void ApplyBloodVisuals(SimpleProjectile projectile)
    {
        Renderer rend = projectile.GetComponentInChildren<Renderer>();
        if (rend != null) rend.material.color = bloodColor;
        
        TrailRenderer trail = projectile.GetComponentInChildren<TrailRenderer>();
        if (trail != null) trail.startColor = bloodColor;
    }
}