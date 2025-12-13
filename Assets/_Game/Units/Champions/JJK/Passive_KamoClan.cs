using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Passives/Kamo Clan")]
public class Passive_KamoClan : PassiveDefinition
{
    [Header("Inventory")]
    public ItemDefinition bloodBagItem;
    public int startingAmount = 3;

    [Header("Blood Technique Constraints")]
    [Range(0f, 1f)] 
    public float minHpPercent = 0.3f; // 30% limit

    public override void OnEquip(UnitStats stats)
    {
        // 1. Existing Inventory Logic
        UnitInventory inventory = stats.GetComponent<UnitInventory>();
        if (inventory == null) inventory = stats.gameObject.AddComponent<UnitInventory>();

        if (bloodBagItem != null)
        {
            inventory.AddItem(bloodBagItem, startingAmount);
        }

        // 2. Apply Restriction to KamoController
        KamoController kamo = stats.GetComponent<KamoController>();
        if (kamo != null)
        {
            kamo.SetBloodThreshold(minHpPercent);
            Debug.Log($"[Passive] Kamo Clan: Blood Limit set to {minHpPercent * 100}% HP.");
        }
    }

    public override void OnUnequip(UnitStats stats)
    {
        // Reset threshold if passive is removed
        KamoController kamo = stats.GetComponent<KamoController>();
        if (kamo != null) kamo.SetBloodThreshold(0f);
    }
}