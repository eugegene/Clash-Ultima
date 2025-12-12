using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Passives/Kamo Clan")]
public class Passive_KamoClan : PassiveDefinition
{
    public ItemDefinition bloodBagItem;
    public int startingAmount = 3;

    public override void OnEquip(UnitStats stats)
    {
        // 1. Get or Add the Inventory Component
        UnitInventory inventory = stats.GetComponent<UnitInventory>();
        if (inventory == null) inventory = stats.gameObject.AddComponent<UnitInventory>();

        // 2. Add the Blood Bags
        if (bloodBagItem != null)
        {
            inventory.AddItem(bloodBagItem, startingAmount);
            Debug.Log($"[Passive] Kamo Clan: Added {startingAmount} Blood Bags.");
        }
    }

    public override void OnUnequip(UnitStats stats)
    {
        // Logic if the passive is removed (rare in MOBAs)
    }
}