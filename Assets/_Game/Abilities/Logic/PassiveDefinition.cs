using UnityEngine;

public abstract class PassiveDefinition : ScriptableObject
{
    [Header("Passive Info")]
    public string passiveName;
    [TextArea] public string description;
    public Sprite icon;

    // Called when the Unit spawns
    public abstract void OnEquip(UnitStats stats);
    
    // Called when the Unit dies or passive is removed
    public abstract void OnUnequip(UnitStats stats);
}