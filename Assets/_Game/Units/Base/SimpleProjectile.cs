using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private float _damage;
    private GameObject _owner;
    private bool _isCrit;
    
    // --- NEW: Homing Support ---
    private UnitStats _homingTarget;

    public void Initialize(Vector3 dir, float speed, float damage, GameObject owner, bool isCrit)
    {
        _direction = dir;
        _speed = speed;
        _damage = damage;
        _owner = owner;
        _isCrit = isCrit;
        Destroy(gameObject, 5f);
    }

    public void SetHomingTarget(UnitStats target)
    {
        _homingTarget = target;
    }

    void Update()
    {
        // If Homing, adjust direction to face target
        if (_homingTarget != null)
        {
            // Aim at chest, not feet
            Vector3 targetCenter = _homingTarget.transform.position + Vector3.up;
            _direction = (targetCenter - transform.position).normalized;
            
            // Optional: Rotate the visual arrow to face the target
            transform.rotation = Quaternion.LookRotation(_direction);
        }

        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner) return;
        if (other.isTrigger) return;

        UnitStats targetStats = other.GetComponentInParent<UnitStats>();
        if (targetStats != null)
        {
            DamageMessage msg = new DamageMessage(_damage, DamageType.Magical, _owner, _isCrit);
            targetStats.TakeDamage(msg);
        }
        Destroy(gameObject);
    }
}