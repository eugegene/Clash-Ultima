using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private float _damage;
    private GameObject _owner;
    private bool _isCrit;

    public void Initialize(Vector3 dir, float speed, float damage, GameObject owner, bool isCrit)
    {
        _direction = dir;
        _speed = speed;
        _damage = damage;
        _owner = owner;
        _isCrit = isCrit;
        Destroy(gameObject, 5f); // Safety timer
    }

    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Ignore the shooter (prevent shooting yourself)
        if (other.gameObject == _owner) return; 

        // 2. Ignore other "Trigger" colliders (like Aggro Ranges) so we don't explode on invisible spheres
        if (other.isTrigger) return;

        // 3. FIX: Use GetComponentInParent to find stats even if we hit a limb/child collider
        UnitStats targetStats = other.GetComponentInParent<UnitStats>();
        
        if (targetStats != null)
        {
            // Deal Damage
            DamageMessage msg = new DamageMessage(_damage, DamageType.Magical, _owner, _isCrit);
            targetStats.TakeDamage(msg);
        }

        // 4. FIX: Destroy on ANY impact (Walls, Ground, Enemies), not just valid targets
        Destroy(gameObject);
    }
}