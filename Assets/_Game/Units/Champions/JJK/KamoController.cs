using UnityEngine;

public class KamoController : MonoBehaviour
{
    [Header("Q: Blood Arrows Settings")]
    public float healthCost = 10f;
    public float bonusDamage = 15f;
    public bool isBloodArrowsActive = false;
    
    [Header("Visuals")]
    public Color bloodColor = Color.red; // Set this to a deep red in Inspector

    private UnitStats _stats;
    private UnitAttack _attack;

    void Awake()
    {
        _stats = GetComponent<UnitStats>();
        _attack = GetComponent<UnitAttack>();
    }

    void OnEnable()
    {
        // Hook into the firing event
        if (_attack != null) _attack.OnProjectileLaunched += ModifyArrow;
    }

    void OnDisable()
    {
        if (_attack != null) _attack.OnProjectileLaunched -= ModifyArrow;
    }

    public void ToggleBloodArrows()
    {
        isBloodArrowsActive = !isBloodArrowsActive;
        Debug.Log($"Blood Arrows Active: {isBloodArrowsActive}");
    }

    private void ModifyArrow(SimpleProjectile projectile)
    {
        // Logic checks: Active? Enough HP?
        if (!isBloodArrowsActive) return;

        if (_stats.CurrentHealth > healthCost)
        {
            // 1. Apply Gameplay Mechanics
            _stats.ModifyHealth(-healthCost);
            
            // Only make it homing if we have a valid target
            if (_attack.currentTarget != null)
            {
                projectile.SetHomingTarget(_attack.currentTarget);
            }

            // 2. Apply Visual "Bloody" Effect
            ApplyBloodVisuals(projectile);
        }
        else
        {
            // Auto-turn off if out of HP
            isBloodArrowsActive = false;
            Debug.Log("Not enough HP for Blood Arrows!");
        }
    }

    private void ApplyBloodVisuals(SimpleProjectile projectile)
    {
        // Change the Mesh Color
        Renderer rend = projectile.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.color = bloodColor;
        }

        // Change the Trail Color (if it has one)
        TrailRenderer trail = projectile.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
        {
            trail.startColor = bloodColor;
            trail.endColor = new Color(bloodColor.r, bloodColor.g, bloodColor.b, 0f); // Fade out
        }
    }
}