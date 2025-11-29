using UnityEngine;
using System.Collections;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance;
    public MobaCamera gameCamera;

    [Header("Settings")]
    public Transform redSpawn;
    public Transform blueSpawn;


    public float respawnTime = 5f;

    void Start()
    {
        // 1. Check if we have a choice saved
        if (GameSession.Instance != null && GameSession.Instance.SelectedCharacter != null)
        {
            SpawnCharacter(GameSession.Instance.SelectedCharacter, Team.Blue);
        }
        else
        {
            Debug.LogWarning("No character selected! (Did you start from Main Menu?)");
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
        // 2. Determine Spawn Point
        Transform spawnPoint = (team == Team.Red) ? redSpawn : blueSpawn;

        // 3. Spawn the Prefab stored in the Definition
        if (def.prefab == null)
        {
            Debug.LogError($"UnitDefinition for {def.unitName} has no Prefab assigned!");
            return;
        }

        GameObject hero = Instantiate(def.prefab, spawnPoint.position, spawnPoint.rotation);
        
        // 4. Initialize Stats
        UnitStats stats = hero.GetComponent<UnitStats>();
        stats.definition = def;
        stats.team = team;
        stats.InitializeStats();

        // 5. Make Camera Follow Hero
        if (gameCamera != null)
        {
            gameCamera.targetToFollow = hero.transform;
        }
    }
}