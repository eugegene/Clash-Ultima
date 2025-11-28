using UnityEngine;

// This is the "Runtime" version of the stats.
// It holds the ACTUAL values that change during the game.
public class UnitStats : MonoBehaviour
{
    [Header("Config")]
    public UnitDefinition definition; // Drag the ScriptableObject here

    [Header("Runtime Stats (Read Only)")]
    // We use the Stat class we created to allow for Buffs/Debuffs
    public Stat Health;
    public Stat MaxHealth;
    public Stat HealthRegen;
    
    public Stat Resource; // Current Mana/Energy
    public Stat MaxResource;
    public Stat ResourceRegen;

    public Stat MoveSpeed;
    public Stat Armor;
    public Stat MagicResist;

    // Current State
    public float CurrentHealth { get; private set; }
    public float CurrentResource { get; private set; }

    private void Awake()
    {
        if (definition != null)
        {
            InitializeStats();
        }
    }

    private void Update()
    {
        Regenerate();
    }

    public void InitializeStats()
    {
        // 1. Initialize Objects
        MaxHealth = new Stat(definition.maxHealth);
        HealthRegen = new Stat(definition.healthRegen);
        
        MaxResource = new Stat(definition.maxResource);
        ResourceRegen = new Stat(definition.resourceRegen);

        MoveSpeed = new Stat(definition.moveSpeed);
        Armor = new Stat(definition.armor);
        MagicResist = new Stat(definition.magicResist);

        // 2. Set Current Values
        CurrentHealth = MaxHealth.Value;
        CurrentResource = MaxResource.Value;
    }

    private void Regenerate()
    {
        // Simple Regen Logic
        if (CurrentHealth < MaxHealth.Value)
        {
            CurrentHealth += HealthRegen.Value * Time.deltaTime;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth.Value);
        }

        if (CurrentResource < MaxResource.Value)
        {
            CurrentResource += ResourceRegen.Value * Time.deltaTime;
            CurrentResource = Mathf.Clamp(CurrentResource, 0, MaxResource.Value);
        }
    }

    // Call this when taking damage
    public void ModifyHealth(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth.Value);
        
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(DamageMessage msg)
    {
        // 1. Calculate how much damage actually gets through armor
        float finalDamage = DamageProcessor.CalculateFinalDamage(this, msg);

        // 2. Apply to Health
        CurrentHealth -= finalDamage;
        
        Debug.Log($"{name} took {finalDamage:F1} ({msg.Type}) damage from {msg.Source.name}. (Raw: {msg.Amount})");

        // 3. Clamp & Die
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
        else if (CurrentHealth > MaxHealth.Value)
        {
            CurrentHealth = MaxHealth.Value;
        }
    }

    private void Die()
    {
        Debug.Log($"{name} has died.");
        // Trigger Death Event here later
    }
}