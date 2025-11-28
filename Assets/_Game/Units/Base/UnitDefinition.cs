using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitDef", menuName = "MOBA/Unit Definition")]
public class UnitDefinition : ScriptableObject
{
    [Header("Identity")]
    public string unitName;
    public Sprite icon;
    
    [Header("Core Stats")]
    public float maxHealth = 100f;
    public float healthRegen = 1.5f;
    
    [Header("Resource (Mana/Energy)")]
    public ResourceType resourceType; // Enum we will create
    public float maxResource = 100f;
    public float resourceRegen = 5f;

    [Header("Offense")]
    public float attackDamage = 50f;
    public float attackRange = 5f;
    public float attackSpeed = 1.0f; // Attacks per second

    [Header("Defense")]
    public float armor = 10f; // Physical Resist
    public float magicResist = 10f; // Magical Resist
    
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 20f;
}

public enum ResourceType
{
    None,
    Mana,
    Energy,
    Rage,
    Fury,
    CursedEnergy
}