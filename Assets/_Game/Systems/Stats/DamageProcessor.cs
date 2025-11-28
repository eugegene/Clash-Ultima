using UnityEngine;

public static class DamageProcessor
{
    public static float CalculateFinalDamage(UnitStats defender, DamageMessage msg)
    {
        if (msg.Type == DamageType.True) return msg.Amount;

        float resistance = 0;

        // 1. Determine which stat resists this damage
        switch (msg.Type)
        {
            case DamageType.Physical:
                resistance = defender.Armor.Value;
                break;
            case DamageType.Magical:
            case DamageType.Fire:
            case DamageType.Water:
                resistance = defender.MagicResist.Value;
                break;
        }

        // 2. MOBA Formula: Damage Reduction Factor
        // Example: 50 Armor -> 100 / 150 = 0.66 (Take 66% damage)
        // Note: This prevents Armor form making you invincible.
        float reductionFactor = 100f / (100f + resistance);

        // 3. Apply
        return msg.Amount * reductionFactor;
    }
}