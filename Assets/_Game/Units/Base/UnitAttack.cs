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

        if (currentTarget == null) return;

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
        // 1. Reset Cooldown (1 / AttackSpeed)
        // Example: 2.0 AttackSpeed = 0.5s cooldown
        attackCooldownTimer = 1f / _stats.AttackSpeed.Value;

        // 2. Deal Damage
        // For melee, we apply immediately. For ranged, you'd spawn a Projectile.
        // We'll create a DamageMessage packet.
        DamageMessage msg = new DamageMessage
        {
            Amount = _stats.AttackDamage.Value,
            Type = DamageType.Physical,
            Source = gameObject
        };

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
}