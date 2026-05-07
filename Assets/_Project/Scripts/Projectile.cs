using UnityEngine;

namespace Tower
{
    [DisallowMultipleComponent]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 14f;
        [SerializeField] private float _hitRadius = 0.3f;
        [SerializeField] private float _maxLifetime = 5f;

        private IDamageable _target;
        private float _damage;
        private float _spawnTime;

        public void Init(IDamageable target, float damage)
        {
            _target = target;
            _damage = damage;
            _spawnTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - _spawnTime >= _maxLifetime) { Destroy(gameObject); return; }
            if (_target == null || !_target.IsAlive) { Destroy(gameObject); return; }

            Vector3 to = _target.Position - transform.position;
            float dist = to.magnitude;

            if (dist <= _hitRadius)
            {
                _target.TakeDamage(_damage);
                Destroy(gameObject);
                return;
            }

            transform.position += to / dist * (_speed * Time.deltaTime);
        }
    }
}
