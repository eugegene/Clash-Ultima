using UnityEngine;
using System.Collections;

public class MinionSpawner : MonoBehaviour
{
    [Header("Core Config")]
    public UnitDefinition unitData; // The Body stats (Health, Dmg)
    public AIDefinition aiData;     // The Brain stats (Multipliers, IQ) <--- NEW
    public Team team = Team.Red;
    public Transform spawnPoint;

    [Header("Wave Settings")]
    public float spawnInterval = 10f;
    public int minionsPerWave = 3;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            for (int i = 0; i < minionsPerWave; i++)
            {
                SpawnMinion();
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnMinion()
    {
        if (unitData == null || unitData.prefab == null) 
        {
            Debug.LogError("MinionSpawner: Missing UnitData or Prefab!");
            return;
        }

        // 1. Spawn
        GameObject minion = Instantiate(unitData.prefab, spawnPoint.position, spawnPoint.rotation);
        
        // 2. Initialize Stats
        UnitStats stats = minion.GetComponent<UnitStats>();
        stats.definition = unitData;
        stats.team = team;
        stats.InitializeStats();

        // 3. Apply AI Modifiers (The Magic Part)
        if (aiData != null)
        {
            ApplyAIModifiers(stats);
            
            // Attach the AI Brain (We will write this next)
            // minion.AddComponent<MinionAI>().Initialize(aiData); 
        }
    }

    private void ApplyAIModifiers(UnitStats stats)
    {
        // Add difficulty stats ON TOP of base stats
        if (aiData.healthMod != 1f)
        {
            float bonus = stats.MaxHealth.BaseValue * (aiData.healthMod - 1f);
            stats.MaxHealth.AddModifier(new StatModifier(bonus, StatModType.Flat, this));
            stats.ModifyHealth(bonus); // Heal the bonus amount
        }

        if (aiData.damageMod != 1f)
        {
            float bonus = stats.AttackDamage.BaseValue * (aiData.damageMod - 1f);
            stats.AttackDamage.AddModifier(new StatModifier(bonus, StatModType.Flat, this));
        }
        
        // You can add Speed, Armor, etc. here
    }
}