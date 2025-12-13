using UnityEngine;

public enum TargetingMode
{
    NoTarget,       
    Point,          
    TargetUnit      
}

public abstract class AbilityDefinition : ScriptableObject
{
    [Header("Display")]
    public string abilityName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Costs & Cooldowns")]
    public float manaCost;
    public float cooldown;
    public float castRange; 

    [Header("Settings")]
    public TargetingMode targetingMode;

    // --- NEW METHOD ---
    // Called once when the game starts (good for Passives in Ability Slots)
    public virtual void OnEquip(UnitStats user) { }
    // ------------------

    public abstract void OnCast(UnitStats caster, Vector3 point, UnitStats target);
}