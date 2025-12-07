using UnityEngine;
using System.Collections;

public class MinionSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public UnitDefinition minionData; // The base stats (e.g., "Skeleton")
    public Team team = Team.Red;
    public Transform spawnPoint;

    [Header("Wave Settings")]
    public float spawnInterval = 10f;
    public int minionsPerWave = 3;
    public bool spawnOnStart = true;

    [Header("Difficulty Modifiers (IQ & Stats)")]
    [Tooltip("Multiplier for HP (e.g. 1.5 = +50% Health)")]
    public float healthMultiplier = 1.0f; 
    
    [Tooltip("Multiplier for Damage")]
    public float damageMultiplier = 1.0f;

    [Tooltip("Aggro Range Modifier (Higher = Smarter/More Aggressive)")]
    public float aggroRangeMultiplier = 1.0f;

    private void Start()
    {
        if (spawnOnStart) StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            for (int i = 0; i < minionsPerWave; i++)
            {
                SpawnMinion();
                yield return new WaitForSeconds(0.5f); // Stagger spawns slightly
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnMinion()
    {
        if (minionData == null || minionData.prefab == null) return;

        // 1. Spawn
        GameObject minionObj = Instantiate(minionData.prefab, spawnPoint.position, spawnPoint.rotation);
        
        // 2. Setup Stats
        UnitStats stats = minionObj.GetComponent<UnitStats>();
        stats.definition = minionData;
        stats.team = team;
        stats.InitializeStats();

        // 3. Apply Difficulty Modifiers (Using your Stat System!)
        if (healthMultiplier != 1.0f)
        {
            // Add a percentage modifier (e.g., 1.5 becomes +0.5 or +50%)
            stats.MaxHealth.AddModifier(new StatModifier(healthMultiplier - 1f, StatModType.PercentAdd, this));
            stats.ModifyHealth(9999); // Heal to new max
        }

        if (damageMultiplier != 1.0f)
        {
            stats.AttackDamage.AddModifier(new StatModifier(damageMultiplier - 1f, StatModType.PercentAdd, this));
        }

        // 4. Apply "IQ" (Aggro Range)
        // We modify the AttackRange stat, allowing them to "see" targets from further away
        if (aggroRangeMultiplier != 1.0f)
        {
            stats.AttackRange.AddModifier(new StatModifier(aggroRangeMultiplier - 1f, StatModType.PercentAdd, this));
        }
    }
}