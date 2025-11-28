using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Abilities/Target Unit")]
public class TargetAbility : AbilityDefinition
{
    [Header("Targeting Logic")]
    public float damage = 30f;
    public GameObject hitVFX; // Explosion/Blood effect

    public override void OnCast(UnitStats caster, Vector3 point, UnitStats target)
    {
        if (target == null) return;

        // NEW: Create Message
        // (In future, we can add a 'DamageType' field to AbilityDefinition to make this configurable)
        DamageMessage msg = new DamageMessage(damage, DamageType.Magical, caster.gameObject);
        
        target.TakeDamage(msg);

        if (hitVFX != null) Instantiate(hitVFX, target.transform.position, Quaternion.identity);
    }
}