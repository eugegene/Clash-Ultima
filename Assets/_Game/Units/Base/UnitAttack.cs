using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitMotor))]
public class UnitAttack : MonoBehaviour
{
    public event System.Action<SimpleProjectile> OnProjectileLaunched;

    private UnitStats _stats;
    private UnitMotor _motor;
    
    [Header("State")]
    public UnitStats currentTarget;
    public float attackCooldownTimer;

    void Awake()
    {
        _stats = GetComponent<UnitStats>();
        _motor = GetComponent<UnitMotor>();
    }

    void OnEnable()
    {
        InputHandler.OnAttackCommand += OnAttackCommandReceived;
        InputHandler.OnMoveCommand += OnMoveCommandReceived;
    }

    void OnDisable()
    {
        InputHandler.OnAttackCommand -= OnAttackCommandReceived;
        InputHandler.OnMoveCommand -= OnMoveCommandReceived;
    }

    // Add a state variable to track what we are doing
    private bool _isAttacking = false;

    void Update()
    {
        if (attackCooldownTimer > 0) 
            attackCooldownTimer -= Time.deltaTime;

        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy) 
        {
            currentTarget = null;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        float range = _stats.AttackRange.Value;

        // --- NEW LOGIC: BUFFER ZONE ---
        // If we are already attacking, allow the enemy to be 20% further away before we chase them
        float stopChaseDistance = _isAttacking ? (range * 1.2f) : range;

        if (distance <= stopChaseDistance)
        {
            // We are close enough to fight
            _isAttacking = true;
            _motor.StopMoving();
            FaceTarget();
            
            if (attackCooldownTimer <= 0)
            {
                PerformAttack();
            }
        }
        else
        {
            // Target is too far, start chasing
            _isAttacking = false;
            _motor.MoveToPoint(currentTarget.transform.position);
        }
    }

    private void PerformAttack()
    {
        attackCooldownTimer = 1f / _stats.AttackSpeed.Value;

        switch (_stats.definition.attackType)
        {
            case AttackType.Touch:
                DoMeleeAttack();
                break;
            case AttackType.Projectile:
                DoRangedAttack();
                break;
            case AttackType.Splash:
                DoSplashAttack();
                break;
        }

        Debug.Log($"Attacked {currentTarget.name}!");
    }

    private void FaceTarget()
    {
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        direction.y = 0; // Keep flat
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 20f);
        }
    }

    // --- EVENTS ---

    private void OnAttackCommandReceived(UnitStats target)
    {
        // Check if it's an enemy
        if (TeamLogic.IsEnemy(_stats.team, target.team))
        {
            currentTarget = target;
        }
    }

    private void OnMoveCommandReceived(Vector3 point)
    {
        // If we order a move, cancel the attack target
        currentTarget = null;
    }

    private float GetDamage(out bool isCrit) // Update helper to return bool
    {
        float damage = _stats.AttackDamage.Value;
        isCrit = false;

        if (Random.value < (_stats.CritChance.Value / 100f))
        {
            damage *= _stats.CritDamage.Value;
            isCrit = true; // We crit!
        }
        return damage;
    }

    private void DoMeleeAttack()
    {
        bool isCrit;
        float dmg = GetDamage(out isCrit);
        
        // Pass the bool into the message
        DamageMessage msg = new DamageMessage(dmg, DamageType.Physical, gameObject, isCrit);
        currentTarget.TakeDamage(msg);
    }

    private void DoRangedAttack()
    {
        if (_stats.definition.projectilePrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up;
        GameObject proj = Instantiate(_stats.definition.projectilePrefab, spawnPos, Quaternion.identity);
        SimpleProjectile pScript = proj.GetComponent<SimpleProjectile>();
        
        if (pScript != null)
        {
            Vector3 targetCenter = currentTarget.transform.position + Vector3.up; 
            Vector3 dir = (targetCenter - spawnPos).normalized;
            bool isCrit;
            float damage = GetDamage(out isCrit); 

            pScript.Initialize(dir, 20f, damage, gameObject, isCrit);
            
            // --- TRIGGER EVENT ---
            OnProjectileLaunched?.Invoke(pScript);
        }
    }

    private void DoSplashAttack()
    {
        Collider[] hits = Physics.OverlapSphere(currentTarget.transform.position, _stats.definition.splashRadius);
        
        // 1. Roll for Crit ONCE for the explosion 
        // (Or move this inside the loop if you want to roll for each enemy individually)
        bool isCrit;
        float damage = GetDamage(out isCrit);

        foreach (var hit in hits)
        {
            UnitStats victim = hit.GetComponent<UnitStats>();
            
            if (victim != null && victim != _stats && TeamLogic.IsEnemy(_stats.team, victim.team))
            {
                // 2. Use the calculated damage and IsCrit flag
                DamageMessage msg = new DamageMessage(damage, DamageType.Fire, gameObject, isCrit);
                victim.TakeDamage(msg);
            }
        }
    }
}