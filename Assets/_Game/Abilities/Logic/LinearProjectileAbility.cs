using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Abilities/Linear Projectile")]
public class LinearProjectileAbility : AbilityDefinition
{
    [Header("Projectile Specifics")]
    public GameObject projectilePrefab; // The sphere/VFX
    public float projectileSpeed = 20f;
    public float projectileDamage = 50f;

    public override void OnCast(UnitStats caster, Vector3 point, UnitStats target)
    {
        // 1. Calculate direction
        Vector3 direction = (point - caster.transform.position).normalized;
        direction.y = 0; // Keep it flat

        // 2. Spawn Position (Chest height, slightly forward)
        Vector3 spawnPos = caster.transform.position + Vector3.up + (direction * 1f);

        // 3. Spawn
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));
        
        // 4. Initialize the Projectile Logic
        // Note: We need a simple Projectile script for the prefab to handle movement/collision.
        // We will define that next.
        SimpleProjectile pScript = proj.GetComponent<SimpleProjectile>();
        if (pScript != null)
        {
            pScript.Initialize(direction, projectileSpeed, projectileDamage, caster.gameObject);
        }
    }
}