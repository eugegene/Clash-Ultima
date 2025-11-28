using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private float _damage;
    private GameObject _owner;

    public void Initialize(Vector3 dir, float speed, float damage, GameObject owner)
    {
        _direction = dir;
        _speed = speed;
        _damage = damage;
        _owner = owner;
        Destroy(gameObject, 5f); // Safety destroy
    }

    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner) return; 

        UnitStats targetStats = other.GetComponent<UnitStats>();
        if (targetStats != null)
        {
            // NEW: Create a Message Packet
            DamageMessage msg = new DamageMessage(_damage, DamageType.Magical, _owner);
            
            // Send it
            targetStats.TakeDamage(msg);
            
            Destroy(gameObject);
        }
    }
}