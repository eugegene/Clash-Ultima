using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Abilities/Self AoE")]
public class SelfAoEAbility : AbilityDefinition
{
    [Header("AoE Settings")]
    public float radius = 5f;
    public float damage = 40f;
    public GameObject explosionVFX;
    public LayerMask targetLayers; // Who gets hit?

    public override void OnCast(UnitStats caster, Vector3 point, UnitStats target)
    {
        // 1. Spawn Visuals at Caster's feet
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, caster.transform.position, Quaternion.identity);
        }

        // 2. Find Targets in Range
        Collider[] hits = Physics.OverlapSphere(caster.transform.position, radius, targetLayers);

        foreach (Collider hit in hits)
        {
            // Don't hit yourself
            if (hit.gameObject == caster.gameObject) continue;

            UnitStats enemy = hit.GetComponent<UnitStats>();
            if (enemy != null)
            {
                enemy.ModifyHealth(-damage);
                Debug.Log($"AoE Hit: {enemy.name}");
            }
        }
    }
}