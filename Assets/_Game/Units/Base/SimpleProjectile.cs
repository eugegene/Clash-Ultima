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
            DamageMessage msg = new DamageMessage(_damage, DamageType.Magical, _owner, _isCrit);
            targetStats.TakeDamage(msg);
            
            Destroy(gameObject);
        }
    }
}