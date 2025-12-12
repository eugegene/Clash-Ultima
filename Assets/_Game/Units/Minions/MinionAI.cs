using UnityEngine;

public class MinionAI : MonoBehaviour
{
    private AIDefinition _aiDef;
    private UnitAttack _attack;
    private UnitStats _stats;
    private float _timer;

    public void Initialize(AIDefinition aiDef)
    {
        _aiDef = aiDef;
        _attack = GetComponent<UnitAttack>();
        _stats = GetComponent<UnitStats>();
        
        // Force an immediate scan on spawn
        ScanForEnemies();
    }

    private void Update()
    {
        if (_aiDef == null || _stats.CurrentHealth <= 0) return;

        _timer += Time.deltaTime;
        
        // Check more frequently (defined by AttackInterval)
        if (_timer >= _aiDef.attackInterval)
        {
            _timer = 0;
            ScanForEnemies();
        }
    }

    private void ScanForEnemies()
    {
        // 1. If we already have a valid target, don't change it (stick to the fight)
        if (_attack.currentTarget != null && 
            _attack.currentTarget.CurrentHealth > 0 &&
            Vector3.Distance(transform.position, _attack.currentTarget.transform.position) <= _aiDef.aggroRange * 1.5f)
        {
            return; 
        }

        // 2. Normal Scan
        Collider[] hits = Physics.OverlapSphere(transform.position, _aiDef.aggroRange);
        UnitStats bestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            UnitStats target = hit.GetComponent<UnitStats>();
            if (ValidateTarget(target))
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    bestTarget = target;
                }
            }
        }

        // 3. GLOBAL FALLBACK: If no one is near, find the Player globally
        if (bestTarget == null)
        {
            // Assuming your Player has the tag "Player"
            GameObject player = GameObject.FindGameObjectWithTag("Player"); 
            if (player != null)
            {
                UnitStats playerStats = player.GetComponent<UnitStats>();
                if (ValidateTarget(playerStats))
                {
                    bestTarget = playerStats;
                }
            }
        }

        if (bestTarget != null)
        {
            _attack.currentTarget = bestTarget;
        }
    }

    private bool ValidateTarget(UnitStats target)
    {
        if (target == null) return false;
        if (target == _stats) return false; // Don't attack self
        if (target.CurrentHealth <= 0) return false; // Don't attack dead
        return TeamLogic.IsEnemy(_stats.team, target.team);
    }
}