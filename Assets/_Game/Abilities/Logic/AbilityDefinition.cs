using UnityEngine;

public enum TargetingMode
{
    NoTarget,       // Instant cast (War Stomp, Self Heal)
    Point,          // Skillshots (Fireball, Blink)
    TargetUnit      // Point & Click (Heal Ally, Execute Enemy)
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
    public float castRange; // How far can I throw it?

    [Header("Settings")]
    public TargetingMode targetingMode;

    // The Magic Function. Every spell must implement this differently.
    // caster: The Hero using it.
    // point: The mouse position on the ground.
    // target: The specific unit clicked (if any).
    public abstract void OnCast(UnitStats caster, Vector3 point, UnitStats target);
}