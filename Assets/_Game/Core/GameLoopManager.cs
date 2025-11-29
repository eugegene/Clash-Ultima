using UnityEngine;
using System.Collections;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance;

    [Header("Settings")]
    public Transform redSpawn;
    public Transform blueSpawn;
    public float respawnTime = 5f;

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
}