using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Abilities/Kamo/Flowing Red Scale")]
public class Passive_FlowingRedScale : PassiveDefinition
{
    [Header("Buff Stats")]
    [Tooltip("Movement Speed % Increase (e.g. 0.20 for 20%)")]
    public float moveSpeedPercent = 0.20f;

    [Tooltip("Attack Speed % Increase (e.g. 0.30 for 30%)")]
    public float attackSpeedPercent = 0.30f;

    [Tooltip("Crit Chance Flat Increase (e.g. 25 for +25% crit)")]
    public float critChanceFlat = 25f;

    public override void OnEquip(UnitStats stats)
    {
        // Add the logic component to the unit
        Buff_FlowingRedScale logic = stats.gameObject.AddComponent<Buff_FlowingRedScale>();
        logic.Initialize(stats, moveSpeedPercent, attackSpeedPercent, critChanceFlat);
    }

    public override void OnUnequip(UnitStats stats)
    {
        Buff_FlowingRedScale logic = stats.GetComponent<Buff_FlowingRedScale>();
        if (logic != null)
        {
            Destroy(logic);
        }
    }

    public override bool IsConditionMet(UnitStats stats)
    {
        // 1. Find the logic component on the player
        var logic = stats.GetComponent<Buff_FlowingRedScale>();

        // 2. If logic exists, return its active state
        // (True = Bright/Active, False = Dark/Inactive)
        if (logic != null)
        {
            return logic.IsActive; 
        }

        // 3. If logic is missing, return FALSE (Dark Overlay)
        Debug.LogWarning("[PassiveCheck] Buff_FlowingRedScale component NOT found on player!");
        return false;
    }
}