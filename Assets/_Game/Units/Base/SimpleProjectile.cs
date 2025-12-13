using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class SimpleProjectile : MonoBehaviour
{
    public float BaseDamage { get; private set; }
    public bool IsCrit { get; private set; }

    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _turnRate = 12f;

    private Vector3 _direction;
    private GameObject _owner;
    private UnitStats _homingTarget;
    private Rigidbody _rb;
    private float _spawnTime;

    // Cached stable aim point
    private Vector3 _aimOffset = new Vector3(0f, 0.9f, 0f);

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = false;

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        _spawnTime = Time.time;
    }

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

    public void SetDamage(float amount)
    {
        BaseDamage = amount;
    }

    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }

    void Update()
    {
        if (Time.time - _spawnTime >= _lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        if (_homingTarget != null && _homingTarget.gameObject.activeInHierarchy)
        {
            Vector3 aimPoint = _homingTarget.transform.position + _aimOffset;

            Vector3 toTarget = aimPoint - transform.position;

            // Prevent backward steering (boomerang fix)
            if (Vector3.Dot(toTarget.normalized, _direction) > 0f)
            {
                Vector3 desiredDir = toTarget.normalized;
                _direction = Vector3.Slerp(
                    _direction,
                    desiredDir,
                    Time.deltaTime * _turnRate
                ).normalized;

                transform.rotation = Quaternion.LookRotation(_direction);
            }
        }

        transform.position += _direction * _speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_owner != null && other.transform.root.gameObject == _owner)
            return;

        UnitStats hit = other.GetComponent<UnitStats>() 
                     ?? other.GetComponentInParent<UnitStats>();

        if (hit != null)
        {
            UnitStats ownerStats = _owner.GetComponent<UnitStats>();
            if (ownerStats != null && !TeamLogic.IsEnemy(ownerStats.team, hit.team))
                return;

            DamageMessage msg = new DamageMessage(
                BaseDamage,
                DamageType.Magical,
                _owner,
                IsCrit
            );

            hit.TakeDamage(msg);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
