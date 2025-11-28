using UnityEngine;

public enum StatModType
{
    Flat = 100,     // Adds directly (e.g., +10 Damage)
    PercentAdd = 200,  // Adds percentage of base (e.g., +10% Strength)
    PercentMult = 300, // Multiplies final result (e.g., Double Damage rune)
}

[System.Serializable]
public class StatModifier
{
    public float Value;
    public StatModType Type;
    public int Order;
    public object Source; // Who applied this? (The item, the spell, etc.)

    public StatModifier(float value, StatModType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    // Constructor without specific order (defaults to type)
    public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
}