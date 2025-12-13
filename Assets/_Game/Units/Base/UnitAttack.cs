// UnitAttack.cs
using UnityEngine;
using System;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitMotor))]
public class UnitAttack : MonoBehaviour
{
    // --- EVENTS ---
    public event Action<UnitStats> OnAttackStarted;
    public event Action<SimpleProjectile> OnProjectileLaunched;
    public event Action<DamageMessage> OnBeforeDamageApplied;
    public event Action<DamageMessage> OnAfterDamageApplied;

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
            _isAttacking = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        float range = _stats.AttackRange.Value;
        float stopChaseDistance = _isAttacking ? range * 1.2f : range;

        if (distance <= stopChaseDistance)
        {
            _isAttacking = true;
            _motor.StopMoving();
            FaceTarget();

            if (attackCooldownTimer <= 0f)
                PerformAttack();
        }
        else
        {
            _isAttacking = false;
            _motor.MoveToPoint(currentTarget.transform.position);
        }
    }

    private void FaceTarget()
    {
        Vector3 dir = currentTarget.transform.position - transform.position;
        dir.y = 0f;
        dir.Normalize();
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 18f);
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
        float dmg = _stats.AttackDamage.Value;
        isCrit = false;

        if (UnityEngine.Random.value < (_stats.CritChance.Value / 100f))
        {
            dmg *= _stats.CritDamage.Value;
            isCrit = true;
        }
        return dmg;
    }

    private void PerformAttack()
    {
        attackCooldownTimer = 1f / Mathf.Max(0.001f, _stats.AttackSpeed.Value);

        // Trigger generic event before attack logic
        OnAttackStarted?.Invoke(currentTarget);

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
    }

    private void DoMeleeAttack()
    {
        bool isCrit;
        float dmg = GetDamage(out isCrit);

        DamageMessage msg = new DamageMessage(dmg, DamageType.Physical, gameObject, isCrit);

        // Event before applying damage
        OnBeforeDamageApplied?.Invoke(msg);

        currentTarget.TakeDamage(msg);

        // Event after applying damage
        OnAfterDamageApplied?.Invoke(msg);
    }

    private void DoRangedAttack()
    {
        if (_stats.definition.projectilePrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * 0.6f;
        Quaternion spawnRot = Quaternion.identity;

        GameObject projObj = Instantiate(_stats.definition.projectilePrefab, spawnPos, spawnRot);
        SimpleProjectile proj = projObj.GetComponent<SimpleProjectile>();

        if (proj != null)
        {
            Vector3 targetCenter = currentTarget.transform.position + Vector3.up * 0.6f;
            Vector3 dir = (targetCenter - spawnPos).normalized;

            bool isCrit;
            float dmg = GetDamage(out isCrit);

            proj.Initialize(dir, 20f, dmg, gameObject, isCrit);

            // Trigger event so external abilities can modify the projectile
            OnProjectileLaunched?.Invoke(proj);
        }
    }

    private void DoSplashAttack()
    {
        Collider[] hits = Physics.OverlapSphere(currentTarget.transform.position, _stats.definition.splashRadius);

        bool isCrit;
        float dmg = GetDamage(out isCrit);

        foreach (var hit in hits)
        {
            UnitStats victim = hit.GetComponent<UnitStats>();
            if (victim == null)
                victim = hit.GetComponentInParent<UnitStats>();

            if (victim != null && victim != _stats && TeamLogic.IsEnemy(_stats.team, victim.team))
            {
                DamageMessage msg = new DamageMessage(dmg, DamageType.Fire, gameObject, isCrit);

                OnBeforeDamageApplied?.Invoke(msg);

                victim.TakeDamage(msg);

                OnAfterDamageApplied?.Invoke(msg);
            }
        }
    }
}
