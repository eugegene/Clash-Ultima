using UnityEngine;

public class MinionAI : MonoBehaviour
{
    private AIDefinition _aiDef;
    private UnitAttack _attack;
    private UnitStats _stats;
    private float _timer;

    // This is called by MinionSpawner to pass the settings
    public void Initialize(AIDefinition aiDef)
    {
        _aiDef = aiDef;
        _attack = GetComponent<UnitAttack>();
        _stats = GetComponent<UnitStats>();

        // Random offset so they don't all scan at the exact same millisecond
        _timer = Random.Range(0, _aiDef.attackInterval);
    }

    private void Update()
    {
        if (_aiDef == null || _stats.CurrentHealth <= 0) return;

        _timer += Time.deltaTime;
        if (_timer >= _aiDef.attackInterval)
        {
            _timer = 0;
            ScanForEnemies();
        }
    }

    private void ScanForEnemies()
    {
        // 1. Look for colliders within "Aggro Range"
        Collider[] hits = Physics.OverlapSphere(transform.position, _aiDef.aggroRange);
        
        UnitStats bestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            UnitStats target = hit.GetComponent<UnitStats>();
            
            // Validate target: Exists, Alive, Not Me, Is Enemy
            if (target != null && target != _stats && target.CurrentHealth > 0)
            {
                if (TeamLogic.IsEnemy(_stats.team, target.team))
                {
                    float dist = Vector3.Distance(transform.position, target.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        bestTarget = target;
                    }
                }
            }
        }

        // 2. If we found a target, tell the Body (UnitAttack) to kill it
        if (bestTarget != null)
        {
            _attack.currentTarget = bestTarget;
        }
    }
}