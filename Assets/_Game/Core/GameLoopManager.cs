using UnityEngine;
using System.Collections;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance;

    [Header("Settings")]
    public Transform redSpawn;
    public Transform blueSpawn;
    public float respawnTime = 5f;

    void Start() // Changed from Awake to Start to ensure GameSession is ready
    {
        // Default to existing if nothing selected (for testing)
        UnitDefinition pick = null;
    
        if (GameSession.Instance != null && GameSession.Instance.SelectedCharacter != null)
        {
            pick = GameSession.Instance.SelectedCharacter;
        }

        if (pick != null)
        {
            SpawnCharacter(pick, Team.Blue);
        }
    }
    
    void Awake()
    {
        Instance = this;
    }

    public void OnUnitDied(UnitStats unit)
    {
        // 1. Hide the unit immediately
        unit.gameObject.SetActive(false);

        // 2. Start the Respawn Timer
        StartCoroutine(RespawnRoutine(unit));
    }

    IEnumerator RespawnRoutine(UnitStats unit)
    {
        Debug.Log($"{unit.name} respawning in {respawnTime}s...");
        yield return new WaitForSeconds(respawnTime);

        // 3. Reset Stats (Heal to full)
        unit.InitializeStats();

        // 4. Find Spawn Point
        Transform spawnPoint = (unit.team == Team.Red) ? redSpawn : blueSpawn;
        
        // 5. Warp to Base (Use Warp() for NavMeshAgents to prevent errors)
        UnityEngine.AI.NavMeshAgent agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) 
        {
            agent.Warp(spawnPoint.position);
        }
        else 
        {
            unit.transform.position = spawnPoint.position;
        }

        // 6. Revive
        unit.gameObject.SetActive(true);
        Debug.Log($"{unit.name} respawned!");
    }

    public void SpawnCharacter(UnitDefinition def, Team team)
    {
        // Instantiate the Prefab from the Definition
        Transform spawnPoint = (team == Team.Red) ? redSpawn : blueSpawn;
        GameObject hero = Instantiate(def.prefab, spawnPoint.position, spawnPoint.rotation);
    
        // Initialize
        UnitStats stats = hero.GetComponent<UnitStats>();
        stats.definition = def; // Ensure data is injected
        stats.team = team;
        stats.InitializeStats();
    
        // Let camera follow
        FindObjectOfType<MobaCamera>().targetToFollow = hero.transform;
    }
}