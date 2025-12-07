using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitMotor))]
public class UnitAttack : MonoBehaviour
{
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

    void Update()
    {
        // Reduce Cooldown
        if (attackCooldownTimer > 0) 
            attackCooldownTimer -= Time.deltaTime;

        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy) 
        {
            currentTarget = null; // Clear the target so we stop chasing ghosts
            return;
        }

        // 1. Check Distance
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        float range = _stats.AttackRange.Value;

        if (distance <= range)
        {
            // In Range: Attack!
            _motor.StopMoving();
            FaceTarget();
            
            if (attackCooldownTimer <= 0)
            {
                PerformAttack();
            }
        }
        else
        {
            // Out of Range: Chase!
            _motor.MoveToPoint(currentTarget.transform.position);
        }
    }

    private void PerformAttack()
    {
        attackCooldownTimer = 1f / _stats.AttackSpeed.Value;
        DamageMessage msg = new DamageMessage
        {
            Amount = _stats.AttackDamage.Value,
            Type = DamageType.Physical,
            Source = gameObject
        };

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

        currentTarget.TakeDamage(msg);

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

    private void DoMeleeAttack()
    {
        // Simple direct damage
        DamageMessage msg = new DamageMessage(_stats.AttackDamage.Value, DamageType.Physical, gameObject);
        currentTarget.TakeDamage(msg);
    }

    private void DoRangedAttack()
    {
        if (_stats.definition.projectilePrefab == null) return;

        // Spawn Projectile
        GameObject proj = Instantiate(_stats.definition.projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
        
        // Setup Projectile
        SimpleProjectile pScript = proj.GetComponent<SimpleProjectile>();
        if (pScript != null)
        {
            Vector3 dir = (currentTarget.transform.position - transform.position).normalized;
            // Use 20f as default speed if not defined, or add projectileSpeed to Definition later
            pScript.Initialize(dir, 20f, _stats.AttackDamage.Value, gameObject);
        }
    }

    private void DoSplashAttack()
    {
        // Boom! Damage everyone near the target
        Collider[] hits = Physics.OverlapSphere(currentTarget.transform.position, _stats.definition.splashRadius);
        
        foreach (var hit in hits)
        {
            UnitStats victim = hit.GetComponent<UnitStats>();
            // Check: Exists, Not Me, Is Enemy
            if (victim != null && victim != _stats && TeamLogic.IsEnemy(_stats.team, victim.team))
            {
                DamageMessage msg = new DamageMessage(_stats.AttackDamage.Value, DamageType.Fire, gameObject);
                victim.TakeDamage(msg);
            }
        }
    }
}