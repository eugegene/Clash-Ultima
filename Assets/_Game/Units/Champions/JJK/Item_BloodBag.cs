using UnityEngine;

[CreateAssetMenu(menuName = "MOBA/Items/Blood Bag")]
public class Item_BloodBag : ItemDefinition
{
    public float healAmount = 50f;

    public override void OnUse(UnitStats user)
    {
        user.ModifyHealth(healAmount);
        Debug.Log($"<color=red>Used Blood Bag!</color> Healed {healAmount}.");
    }
}