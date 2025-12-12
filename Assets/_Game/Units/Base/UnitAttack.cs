// UnitAttack.cs
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

    private bool _isAttacking = false;

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
        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        float range = _stats.AttackRange.Value;

        float stopChaseDistance = _isAttacking ? (range * 1.2f) : range;

        if (distance <= stopChaseDistance)
        {
            _isAttacking = true;
            _motor.StopMoving();
            FaceTarget();

            if (attackCooldownTimer <= 0f)
            {
                PerformAttack();
            }
        }
        else
        {
            _isAttacking = false;
            _motor.MoveToPoint(currentTarget.transform.position);
        }
    }

    private void PerformAttack()
    {
        attackCooldownTimer = 1f / Mathf.Max(0.0001f, _stats.AttackSpeed.Value);

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
        Vector3 direction = (currentTarget.transform.position - transform.position);
        direction.y = 0f;
        direction = direction.normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 20f);
        }
    }

    private void OnAttackCommandReceived(UnitStats target)
    {
        if (TeamLogic.IsEnemy(_stats.team, target.team))
            currentTarget = target;
    }

    private void OnMoveCommandReceived(Vector3 point)
    {
        currentTarget = null;
    }

    private float GetDamage(out bool isCrit)
    {
        float damage = _stats.AttackDamage.Value;
        isCrit = false;

        if (Random.value < (_stats.CritChance.Value / 100f))
        {
            damage *= _stats.CritDamage.Value;
            isCrit = true;
        }
        return damage;
    }

    private void DoMeleeAttack()
    {
        bool isCrit;
        float dmg = GetDamage(out isCrit);
        DamageMessage msg = new DamageMessage(dmg, DamageType.Physical, gameObject, isCrit);
        currentTarget.TakeDamage(msg);
    }

    private void DoRangedAttack()
    {
        if (_stats.definition.projectilePrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * 0.6f;
        Quaternion spawnRot = Quaternion.identity;
        GameObject proj = Instantiate(_stats.definition.projectilePrefab, spawnPos, spawnRot);
        SimpleProjectile pScript = proj.GetComponent<SimpleProjectile>();

        if (pScript != null)
        {
            Vector3 targetCenter = currentTarget.transform.position + Vector3.up * 0.6f;
            Vector3 dir = (targetCenter - spawnPos).normalized;

            bool isCrit;
            float damage = GetDamage(out isCrit);

            pScript.Initialize(dir, 20f, damage, gameObject, isCrit);

            // Notify listeners so they can modify projectile (e.g., KamoController)
            OnProjectileLaunched?.Invoke(pScript);
        }
    }

    private void DoSplashAttack()
    {
        Collider[] hits = Physics.OverlapSphere(currentTarget.transform.position, _stats.definition.splashRadius);

        bool isCrit;
        float damage = GetDamage(out isCrit);

        foreach (var hit in hits)
        {
            UnitStats victim = hit.GetComponent<UnitStats>();
            if (victim == null) victim = hit.GetComponentInParent<UnitStats>();
            if (victim != null && victim != _stats && TeamLogic.IsEnemy(_stats.team, victim.team))
            {
                DamageMessage msg = new DamageMessage(damage, DamageType.Fire, gameObject, isCrit);
                victim.TakeDamage(msg);
            }
        }
    }
}
