using UnityEngine;
using System; // Required for Action event

public class UnitStats : MonoBehaviour
{
    [Header("Config")]
    public UnitDefinition definition;

    [Header("Runtime Stats")]
    public Stat Health;
    public Stat MaxHealth;
    public Stat HealthRegen;
    public Stat Resource;
    public Stat MaxResource;
    public Stat ResourceRegen;
    
    // --- NEW STATS ---
    public Stat AttackDamage;
    public Stat AttackRange;
    public Stat AttackSpeed;
    public Stat CritChance; 
    public Stat CritDamage;
    // -----------------

    public Stat MoveSpeed;
    public Stat Armor;
    public Stat MagicResist;

    [Header("Identity")]
    public Team team;

    public float CurrentHealth { get; private set; }
    public float CurrentResource { get; private set; }

    // --- EVENTS ---
    // Added to allow passives (like Flowing Red Scale) to detect combat
    public event Action<DamageMessage> OnDamageTaken; 
    // --------------

    private void Awake()
    {
        if (definition != null) InitializeStats();
    }

    private void Update()
    {
        Regenerate();
    }

    public void InitializeStats()
    {
        MaxHealth = new Stat(definition.maxHealth);
        HealthRegen = new Stat(definition.healthRegen);
        MaxResource = new Stat(definition.maxResource);
        ResourceRegen = new Stat(definition.resourceRegen);
        
        // --- INIT NEW STATS ---
        AttackDamage = new Stat(definition.attackDamage);
        AttackRange = new Stat(definition.attackRange);
        AttackSpeed = new Stat(definition.attackSpeed);
        CritChance = new Stat(definition.critChance);
        CritDamage = new Stat(definition.critDamage);
        // ----------------------

        MoveSpeed = new Stat(definition.moveSpeed);
        Armor = new Stat(definition.armor);
        MagicResist = new Stat(definition.magicResist);

        CurrentHealth = MaxHealth.Value;
        CurrentResource = MaxResource.Value;
    }
    
    private void Regenerate()
    {
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

    public void ModifyHealth(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth.Value);
        if (CurrentHealth <= 0) Die();
    }

    public void ModifyResource(float amount)
    {
        CurrentResource += amount;
        CurrentResource = Mathf.Clamp(CurrentResource, 0, MaxResource.Value);
    }

    public void TakeDamage(DamageMessage msg)
    {
        float finalDamage = DamageProcessor.CalculateFinalDamage(this, msg);
        CurrentHealth -= finalDamage;
        
        if (DamageTextManager.Instance != null)
        {
            // Pass the IsCrit flag to the UI
            DamageTextManager.Instance.ShowDamage(finalDamage, transform.position, msg.IsCrit);
        }

        // --- NOTIFY LISTENERS ---
        // This triggers the Flowing Red Scale passive "In Combat" state
        OnDamageTaken?.Invoke(msg);
        // ------------------------

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        if (GameLoopManager.Instance != null) GameLoopManager.Instance.OnUnitDied(this);
        else Destroy(gameObject);
    }
}