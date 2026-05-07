using System;
using UnityEngine;

namespace Tower
{
    [DisallowMultipleComponent]
    public sealed class Enemy : MonoBehaviour, IDamageable
    {
        public enum EnemyType { Melee, Ranged }

        [Header("Type")]
        [SerializeField] private EnemyType _type = EnemyType.Melee;

        [Header("Base stats (pre-scaling)")]
        [SerializeField] private float _baseHP = 20f;
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _attackSpeed = 1f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackRange = 1f;
        [SerializeField] private int _goldDrop = 2;

        [Header("Ranged only")]
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Transform _projectileParent;

        public event Action<Enemy> OnDied;

        private Health _health;
        private IDamageable _target;
        private float _runtimeDamage;
        private float _attackCooldown;
        private bool _initialized;

        public Health Logic => _health;
        public EnemyType Type => _type;
        public int GoldDrop => _goldDrop;
        public Vector3 Position => transform.position;
        public bool IsAlive => _health != null && !_health.IsDead;

        public void Init(IDamageable target, float hpMul, float dmgMul, Transform projectileParent = null)
        {
            _target = target;
            _health = new Health(Mathf.Max(1f, _baseHP * hpMul));
            _runtimeDamage = _baseDamage * dmgMul;
            _attackCooldown = 0f;
            _health.OnDied += HandleDeath;
            if (projectileParent != null) _projectileParent = projectileParent;
            _initialized = true;
        }

        public void TakeDamage(float amount) => _health?.TakeDamage(amount);

        private void Update()
        {
            if (!_initialized || _health == null || _health.IsDead) return;
            if (_target == null || !_target.IsAlive) return;

            Vector3 to = _target.Position - transform.position;
            float dist = to.magnitude;

            if (dist > _attackRange)
            {
                if (dist > 0.001f)
                    transform.position += to / dist * (_moveSpeed * Time.deltaTime);
                return;
            }

            if (_attackCooldown > 0f) { _attackCooldown -= Time.deltaTime; return; }
            Attack();
            _attackCooldown = _attackSpeed > 0f ? 1f / _attackSpeed : 1f;
        }

        private void Attack()
        {
            if (_type == EnemyType.Ranged && _projectilePrefab != null)
            {
                var p = Instantiate(_projectilePrefab, transform.position, Quaternion.identity, _projectileParent);
                p.Init(_target, _runtimeDamage);
            }
            else
            {
                _target.TakeDamage(_runtimeDamage);
            }
        }

        private void HandleDeath()
        {
            OnDied?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
