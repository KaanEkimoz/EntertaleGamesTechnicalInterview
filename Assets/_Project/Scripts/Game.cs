using UnityEngine;

namespace Tower
{
    [DisallowMultipleComponent]
    public sealed class Game : MonoBehaviour
    {
        public enum State { Home, Run, GameOver }

        [Header("Refs")]
        [SerializeField] private TowerController _tower;
        [SerializeField] private WaveSystem _waveSystem;
        [SerializeField] private HUD _hud;
        [SerializeField] private HomePanel _homePanel;
        [SerializeField] private GameOverPanel _gameOverPanel;

        [Header("Base stats")]
        [SerializeField] private TowerStats _towerBaseStats = TowerStats.Default;

        public State Current { get; private set; } = State.Home;

        private void Awake()
        {
            if (_tower != null) _tower.OnDied += HandleTowerDied;
            if (_waveSystem != null) _waveSystem.OnEnemyKilled += HandleEnemyKilled;
            ShowHome();
        }

        private void OnDestroy()
        {
            if (_tower != null) _tower.OnDied -= HandleTowerDied;
            if (_waveSystem != null) _waveSystem.OnEnemyKilled -= HandleEnemyKilled;
        }

        public void OnPlayClicked() => StartRun();
        public void OnHomeClicked() => ShowHome();

        private void StartRun()
        {
            var finalStats = UpgradeStore.ApplyTo(_towerBaseStats);
            _tower.Initialize(finalStats);
            _tower.HealToFull();
            _tower.Activate();
            _waveSystem.StartFromWave1();
            Current = State.Run;
            Time.timeScale = 1f;
            if (_homePanel != null) _homePanel.gameObject.SetActive(false);
            if (_gameOverPanel != null) _gameOverPanel.gameObject.SetActive(false);
            if (_hud != null) _hud.gameObject.SetActive(true);
        }

        private void ShowHome()
        {
            Time.timeScale = 1f;
            if (_tower != null) _tower.Deactivate();
            if (_waveSystem != null) _waveSystem.Stop();
            Current = State.Home;
            if (_gameOverPanel != null) _gameOverPanel.gameObject.SetActive(false);
            if (_hud != null) _hud.gameObject.SetActive(false);
            if (_homePanel != null) { _homePanel.gameObject.SetActive(true); _homePanel.Refresh(); }
        }

        private void HandleTowerDied()
        {
            Current = State.GameOver;
            Time.timeScale = 0f;
            if (_gameOverPanel != null) _gameOverPanel.gameObject.SetActive(true);
        }

        private void HandleEnemyKilled(Enemy e)
        {
            if (e != null) Wallet.Add(e.GoldDrop);
        }
    }
}
