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
}