// KamoController.cs
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitAttack))]
public class KamoController : MonoBehaviour
{
    [Header("Q: Blood Arrows Settings")]
    public float healthCost = 10f;
    public float energyCost = 5f; 
    public float bonusDamage = 15f;
    public float bloodArrowSpeed = 35f;

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
        if (_attack != null)
            _attack.OnProjectileLaunched += ModifyArrow;
    }

    void OnDisable()
    {
        if (_attack != null)
            _attack.OnProjectileLaunched -= ModifyArrow;
    }

    public void ToggleBloodArrows()
    {
        isBloodArrowsActive = !isBloodArrowsActive;
    }

    private void ModifyArrow(SimpleProjectile projectile)
    {
        if (!isBloodArrowsActive || projectile == null) return;

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
        if (r != null)
            r.material.color = bloodColor;

        TrailRenderer t = projectile.GetComponentInChildren<TrailRenderer>();
        if (t != null)
            t.startColor = bloodColor;
    }
}
