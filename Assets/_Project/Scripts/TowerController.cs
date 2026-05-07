using System;
using UnityEngine;

namespace Tower
{
    [DisallowMultipleComponent]
    public sealed class TowerController : MonoBehaviour, IDamageable
    {
        [SerializeField] private TowerStats _baseStats = TowerStats.Default;
        [SerializeField] private float _range = 8f;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Transform _projectileParent;

        public event Action OnDied;
        public event Action<float> OnHealthChanged;

        private Health _health;
        private TowerStats _runtimeStats;
        private float _attackCooldown;
        private bool _isActive;
        private static readonly Collider2D[] _hitBuffer = new Collider2D[32];

        public Health Logic => _health;
        public TowerStats RuntimeStats => _runtimeStats;
        public float Range => _range;
        public Vector3 Position => transform.position;
        public bool IsAlive => _health != null && !_health.IsDead;

        public void Initialize(TowerStats finalStats)
        {
            _runtimeStats = finalStats;
            if (_health == null)
            {
                _health = new Health(finalStats.MaxHP);
                _health.OnHealthChanged += n => OnHealthChanged?.Invoke(n);
                _health.OnDied += () => OnDied?.Invoke();
            }
            else
            {
                _health.SetMax(finalStats.MaxHP, addDelta: false);
                _health.HealToFull();
            }
            _attackCooldown = 0f;
        }

        public void Activate() => _isActive = true;
        public void Deactivate() => _isActive = false;
        public void HealToFull() => _health?.HealToFull();

        public void TakeDamage(float amount) => _health?.TakeDamage(amount);

        private void Update()
        {
            if (!_isActive || _health == null || _health.IsDead) return;

            float dt = Time.deltaTime;
            _health.Tick(_runtimeStats.Regen, dt);

            if (_attackCooldown > 0f) _attackCooldown -= dt;
            if (_attackCooldown > 0f) return;

            var target = FindNearestEnemyInRange();
            if (target == null) return;

            Fire(target);
            _attackCooldown = _runtimeStats.AttackSpeed > 0f ? 1f / _runtimeStats.AttackSpeed : 1f;
        }

        private IDamageable FindNearestEnemyInRange()
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, _range, _hitBuffer);
            IDamageable best = null;
            float bestSqr = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                var col = _hitBuffer[i];
                if (col == null) continue;
                if (!col.CompareTag("Enemy")) continue;
                var dmg = col.GetComponentInParent<IDamageable>();
                if (dmg == null || !dmg.IsAlive) continue;
                float sqr = (dmg.Position - transform.position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = dmg;
                }
            }
            return best;
        }

        private void Fire(IDamageable target)
        {
            Vector3 origin = _muzzle != null ? _muzzle.position : transform.position;
            if (_projectilePrefab != null)
            {
                var p = Instantiate(_projectilePrefab, origin, Quaternion.identity, _projectileParent);
                p.Init(target, _runtimeStats.Damage);
            }
            else
            {
                Debug.Log($"[Tower] Fire (no projectile prefab) → dmg {_runtimeStats.Damage}", this);
                target.TakeDamage(_runtimeStats.Damage);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
#endif
    }
}
