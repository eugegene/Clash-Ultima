using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private float _damage;
    private GameObject _owner;
    private bool _isCrit;
    
    private UnitStats _homingTarget;

    public void Initialize(Vector3 dir, float speed, float damage, GameObject owner, bool isCrit)
    {
        _direction = dir;
        _speed = speed;
        _damage = damage;
        _owner = owner;
        _isCrit = isCrit;
        
        // FIX: Rotate immediately to face direction
        if (_direction != Vector3.zero) 
            transform.rotation = Quaternion.LookRotation(_direction);

        Destroy(gameObject, 5f);
    }

    public void SetHomingTarget(UnitStats target)
    {
        _homingTarget = target;
    }

    public void SetDamage(float amount)
    {
        _damage = amount;
    }

    void Update()
    {
        if (_homingTarget != null && _homingTarget.transform != null)
        {
            Vector3 targetCenter = _homingTarget.transform.position + Vector3.up;
            _direction = (targetCenter - transform.position).normalized;
            
            if (_direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_direction);
        }

        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Ignore Shooter & Triggers
        if (other.gameObject == _owner) return;
        if (other.isTrigger) return; 

        // 2. CRITICAL FIX: Look in PARENT for stats (Handles hitting limbs/armor)
        UnitStats targetStats = other.GetComponentInParent<UnitStats>();
        
        if (targetStats != null)
        {
            DamageMessage msg = new DamageMessage(_damage, DamageType.Magical, _owner, _isCrit);
            targetStats.TakeDamage(msg);
        }

        // 3. CRITICAL FIX: Destroy is OUTSIDE the if statement.
        // This ensures the arrow is destroyed even if it hits a wall or a part of the enemy that returns null stats.
        Destroy(gameObject);
    }
}