using UnityEngine;
using UnityEngine.UI;

namespace Tower
{
    [DisallowMultipleComponent]
    public sealed class HUD : MonoBehaviour
    {
        [Header("Tower")]
        [SerializeField] private TowerController _tower;
        [SerializeField] private Text _towerHpText;

        [Header("Wave / Gold")]
        [SerializeField] private WaveSystem _waveSystem;
        [SerializeField] private Text _waveText;
        [SerializeField] private Text _goldText;
        [SerializeField] private Text _breatherText;

        [Header("Stat Levels")]
        [SerializeField] private Text _hpLevelText;
        [SerializeField] private Text _dmgLevelText;
        [SerializeField] private Text _atkSpdLevelText;
        [SerializeField] private Text _regenLevelText;

        private void OnEnable()
        {
            if (_tower != null) _tower.OnHealthChanged += OnTowerHpChanged;
            if (_waveSystem != null)
            {
                _waveSystem.OnWaveStarted += OnWaveStarted;
                _waveSystem.OnBreatherTick += OnBreatherTick;
                _waveSystem.OnBreatherEnded += OnBreatherEnded;
            }
            Wallet.OnGoldChanged += OnGoldChanged;
            UpgradeStore.OnLevelChanged += OnLevelChanged;

            RefreshAll();
        }

        private void OnDisable()
        {
            if (_tower != null) _tower.OnHealthChanged -= OnTowerHpChanged;
            if (_waveSystem != null)
            {
                _waveSystem.OnWaveStarted -= OnWaveStarted;
                _waveSystem.OnBreatherTick -= OnBreatherTick;
                _waveSystem.OnBreatherEnded -= OnBreatherEnded;
            }
            Wallet.OnGoldChanged -= OnGoldChanged;
            UpgradeStore.OnLevelChanged -= OnLevelChanged;
        }

        public void RefreshAll()
        {
            RefreshHp();
            RefreshGold();
            RefreshWave();
            RefreshLevels();
            if (_breatherText != null) _breatherText.gameObject.SetActive(false);
        }

        private void Update() { RefreshHp(); }

        private void RefreshHp()
        {
            if (_towerHpText == null || _tower == null || _tower.Logic == null) return;
            _towerHpText.text = $"{_tower.Logic.Current:F0}/{_tower.Logic.Max:F0}";
        }

        private void OnTowerHpChanged(float _) => RefreshHp();

        private void RefreshGold()
        {
            if (_goldText != null) _goldText.text = $"Gold: {Wallet.Gold}";
        }
        private void OnGoldChanged(int _) => RefreshGold();

        private void RefreshWave()
        {
            if (_waveText != null && _waveSystem != null)
                _waveText.text = $"Wave {_waveSystem.CurrentWave}";
        }
        private void OnWaveStarted(int n) => RefreshWave();

        private void OnBreatherTick(int sec)
        {
            if (_breatherText == null) return;
            _breatherText.gameObject.SetActive(true);
            int next = (_waveSystem != null ? _waveSystem.CurrentWave : 0) + 1;
            _breatherText.text = $"Wave {next} in {sec}...";
        }

        private void OnBreatherEnded()
        {
            if (_breatherText != null) _breatherText.gameObject.SetActive(false);
        }

        private void RefreshLevels()
        {
            SetLv(_hpLevelText,      UpgradeStore.Stat.HP);
            SetLv(_dmgLevelText,     UpgradeStore.Stat.Damage);
            SetLv(_atkSpdLevelText,  UpgradeStore.Stat.AttackSpeed);
            SetLv(_regenLevelText,   UpgradeStore.Stat.Regen);
        }

        private void OnLevelChanged(UpgradeStore.Stat _, int __) => RefreshLevels();

        private static void SetLv(Text t, UpgradeStore.Stat s)
        {
            if (t == null) return;
            t.text = $"{UpgradeStore.ShortName(s)} Lv.{UpgradeStore.GetLevel(s)}";
        }
    }
}
