using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Abilities/General/Passive Slot Adapter")]
public class Ability_PassiveSlot : AbilityDefinition
{
    [Header("Passive Logic")]
    [Tooltip("Drag your Passive asset here (e.g., Passive_FlowingRedScale)")]
    public PassiveDefinition passiveToEquip;

    public override void OnEquip(UnitStats user)
    {
        if (passiveToEquip != null)
        {
            // Delegate the logic to the existing Passive definition
            passiveToEquip.OnEquip(user);
            Debug.Log($"[Ability] Equipped Passive: {passiveToEquip.name} in ability slot.");
        }
    }

    public override void OnCast(UnitStats caster, Vector3 point, UnitStats target)
    {
        // Passives in slots usually don't do anything when pressed.
        // You can add a sound effect or a small "Passive is active" message here if you want.
        Debug.Log($"{abilityName} is a passive ability and cannot be cast manually.");
    }
}