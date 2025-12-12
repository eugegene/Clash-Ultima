// SimpleProjectile.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class SimpleProjectile : MonoBehaviour
{
    // Public read-only for external systems (KamoController)
    public float BaseDamage { get; private set; }
    public bool IsCrit { get; private set; }

    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _proximityDestroy = 0.25f; // kills if close but didn't collide

    private Vector3 _direction;
    private GameObject _owner;
    private UnitStats _homingTarget;
    private Rigidbody _rb;
    private float _spawnTime;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) _rb = gameObject.AddComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = false;

        Collider col = GetComponent<Collider>();
        if (col == null) col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;

        _spawnTime = Time.time;
    }

    /// <summary>
    /// Initialize projectile. dir should be normalized.
    /// </summary>
    public void Initialize(Vector3 dir, float speed, float damage, GameObject owner, bool isCrit)
    {
        _direction = dir.normalized;
        _speed = speed;
        BaseDamage = damage;
        IsCrit = isCrit;
        _owner = owner;

        if (_direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_direction);
    }

    public void SetHomingTarget(UnitStats target)
    {
        _homingTarget = target;
    }

    /// <summary>
    /// Overwrite damage entirely (used sparingly). Prefer to use BaseDamage when adding bonuses.
    /// </summary>
    public void SetDamage(float amount)
    {
        BaseDamage = amount;
    }

    void Update()
    {
        // Lifetime guard
        if (Time.time - _spawnTime >= _lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        // Homing behavior (if target assigned)
        if (_homingTarget != null && _homingTarget.gameObject.activeInHierarchy)
        {
            Vector3 aimPoint = GetHomingAimPoint(_homingTarget);
            Vector3 desiredDir = (aimPoint - transform.position).normalized;
            if (desiredDir != Vector3.zero)
            {
                _direction = Vector3.Slerp(_direction, desiredDir, Time.deltaTime * 15f).normalized;
                transform.rotation = Quaternion.LookRotation(_direction);
            }

            // Proximity kill to avoid eternal orbit in corner cases
            float dist = Vector3.Distance(transform.position, aimPoint);
            if (dist <= _proximityDestroy)
            {
                // Attempt a final damage application by performing a manual overlap check
                TryApplyDamageToTarget(_homingTarget);
                Destroy(gameObject);
                return;
            }
        }

        // Move
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    private Vector3 GetHomingAimPoint(UnitStats target)
    {
        // Prefer collider bounds center if available, otherwise use a sensible chest offset
        Collider c = target.GetComponentInChildren<Collider>();
        if (c != null)
            return c.bounds.center;
        return target.transform.position + Vector3.up * 0.6f;
    }

    private void TryApplyDamageToTarget(UnitStats targetStats)
    {
        if (targetStats == null) return;

        UnitStats ownerStats = _owner != null ? _owner.GetComponent<UnitStats>() : null;
        // Only damage enemies (safe default)
        if (ownerStats != null && !TeamLogic.IsEnemy(ownerStats.team, targetStats.team)) return;

        DamageMessage msg = new DamageMessage(BaseDamage, DamageType.Magical, _owner, IsCrit);
        targetStats.TakeDamage(msg);
    }

    void OnTriggerEnter(Collider other)
    {
        // Safety: ignore nulls
        if (other == null) return;

        // Ignore owner's entire root (all children)
        if (_owner != null && other.transform.root.gameObject == _owner) return;

        // If this collider belongs to a Unit, fetch its UnitStats
        UnitStats hitStats = other.GetComponent<UnitStats>();
        if (hitStats == null)
            hitStats = other.GetComponentInParent<UnitStats>();

        if (hitStats != null)
        {
            // Only damage enemies (avoid friendly fire by default)
            UnitStats ownerStats = _owner != null ? _owner.GetComponent<UnitStats>() : null;
            if (ownerStats == null || TeamLogic.IsEnemy(ownerStats.team, hitStats.team))
            {
                DamageMessage msg = new DamageMessage(BaseDamage, DamageType.Magical, _owner, IsCrit);
                hitStats.TakeDamage(msg);
                Destroy(gameObject);
                return;
            }
            else
            {
                // Friendly hit: ignore
                return;
            }
        }

        // Ignore trigger-only volumes (camera, triggers, etc.)
        if (other.isTrigger) return;

        // Hit world or obstacle
        Destroy(gameObject);
    }
}
