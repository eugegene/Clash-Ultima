using UnityEngine;

public enum DamageType
{
    Physical,   // Blocked by Armor
    Magical,    // Blocked by Magic Resist
    True,       // Ignores everything
    Fire,       // Elemental
    Water,
    Poison
}

// A package containing all info about an incoming attack
public struct DamageMessage
{
    public float Amount;
    public DamageType Type;
    public GameObject Source; // Who attacked?

    public DamageMessage(float amount, DamageType type, GameObject source)
    {
        Amount = amount;
        Type = type;
        Source = source;
    }
}