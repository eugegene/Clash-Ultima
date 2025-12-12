using UnityEngine;

public abstract class ItemDefinition : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;

    // What happens when I press the button?
    public abstract void OnUse(UnitStats user);
}