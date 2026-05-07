using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tower
{
    [DisallowMultipleComponent]
    public sealed class WaveSystem : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private TowerController _tower;
        [SerializeField] private Transform _spawnAround;
        [SerializeField] private Enemy _meleePrefab;
        [SerializeField] private Enemy _rangedPrefab;
        [SerializeField] private Transform _enemyContainer;
        [SerializeField] private Transform _projectileContainer;

        [Header("Tuning")]
        [SerializeField] private float _ringRadius = 12f;
        [SerializeField] private float _breatherSeconds = 3f;

        public event Action<int> OnWaveStarted;
        public event Action<int> OnBreatherTick;
        public event Action OnBreatherEnded;
        public event Action<Enemy> OnEnemyKilled;

        private readonly List<Enemy> _alive = new List<Enemy>();
        private int _currentWave;
        private bool _running;
        private Coroutine _breatherCoroutine;

        public int CurrentWave => _currentWave;
        public bool IsRunning => _running;

        public void StartFromWave1()
        {
            Stop();
            _currentWave = 0;
            _running = true;
            SpawnNextWave();
        }

        public void Stop()
        {
            _running = false;
            if (_breatherCoroutine != null) { StopCoroutine(_breatherCoroutine); _breatherCoroutine = null; }
            for (int i = _alive.Count - 1; i >= 0; i--)
            {
                if (_alive[i] != null) Destroy(_alive[i].gameObject);
            }
            _alive.Clear();
        }

        private void SpawnNextWave()
        {
            _currentWave++;
            int melee, ranged;
            GetCounts(_currentWave, out melee, out ranged);
            float hpMul = 1f + 0.15f * _currentWave;
            float dmgMul = 1f + 0.10f * _currentWave;
            OnWaveStarted?.Invoke(_currentWave);
            for (int i = 0; i < melee; i++) SpawnEnemy(_meleePrefab, hpMul, dmgMul);
            for (int i = 0; i < ranged; i++) SpawnEnemy(_rangedPrefab, hpMul, dmgMul);
        }

        private void SpawnEnemy(Enemy prefab, float hpMul, float dmgMul)
        {
            if (prefab == null) return;
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            Vector3 origin = _spawnAround != null ? _spawnAround.position : Vector3.zero;
            Vector3 pos = origin + new Vector3(Mathf.Cos(angle) * _ringRadius, Mathf.Sin(angle) * _ringRadius, 0f);
            var e = Instantiate(prefab, pos, Quaternion.identity, _enemyContainer);
            e.Init(_tower, hpMul, dmgMul, _projectileContainer);
            e.OnDied += HandleEnemyDied;
            _alive.Add(e);
        }

        private void HandleEnemyDied(Enemy e)
        {
            _alive.Remove(e);
            OnEnemyKilled?.Invoke(e);
            if (!_running) return;
            if (_alive.Count == 0) _breatherCoroutine = StartCoroutine(BreatherRoutine());
        }

        private IEnumerator BreatherRoutine()
        {
            int seconds = Mathf.Max(1, Mathf.CeilToInt(_breatherSeconds));
            for (int s = seconds; s > 0; s--)
            {
                OnBreatherTick?.Invoke(s);
                yield return new WaitForSeconds(1f);
            }
            OnBreatherEnded?.Invoke();
            _breatherCoroutine = null;
            if (_running) SpawnNextWave();
        }

        private static void GetCounts(int wave, out int melee, out int ranged)
        {
            if (wave == 1) { melee = 3; ranged = 1; }
            else if (wave <= 4) { melee = 5; ranged = 2; }
            else { melee = 7; ranged = 3; }
        }
    }
}
