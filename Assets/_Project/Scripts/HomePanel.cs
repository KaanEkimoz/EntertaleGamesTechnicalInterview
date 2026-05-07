using UnityEngine;
using UnityEngine.UI;

namespace Tower
{
    [DisallowMultipleComponent]
    public sealed class HomePanel : MonoBehaviour
    {
        [System.Serializable]
        public sealed class UpgradeRow
        {
            public UpgradeStore.Stat Stat;
            public Button Button;
            public Text Label;
        }

        [SerializeField] private Game _game;
        [SerializeField] private Text _goldText;
        [SerializeField] private Button _playButton;
        [SerializeField] private UpgradeRow[] _rows;

        private void Awake()
        {
            if (_playButton != null) _playButton.onClick.AddListener(OnPlayClicked);
            if (_rows != null)
            {
                foreach (var row in _rows)
                {
                    if (row == null || row.Button == null) continue;
                    var captured = row;
                    row.Button.onClick.AddListener(() => OnUpgradeClicked(captured));
                }
            }
        }

        private void OnEnable()
        {
            Wallet.OnGoldChanged += OnGoldChanged;
            UpgradeStore.OnLevelChanged += OnLevelChanged;
            Refresh();
        }

        private void OnDisable()
        {
            Wallet.OnGoldChanged -= OnGoldChanged;
            UpgradeStore.OnLevelChanged -= OnLevelChanged;
        }

        public void Refresh()
        {
            if (_goldText != null) _goldText.text = $"Gold: {Wallet.Gold}";
            if (_rows == null) return;
            foreach (var row in _rows) UpdateRow(row);
        }

        private void OnGoldChanged(int _) => Refresh();
        private void OnLevelChanged(UpgradeStore.Stat _, int __) => Refresh();

        private void UpdateRow(UpgradeRow row)
        {
            if (row == null || row.Label == null) return;
            int lv = UpgradeStore.GetLevel(row.Stat);
            int cost = UpgradeStore.GetCost(row.Stat);
            row.Label.text = $"{UpgradeStore.IncrementLabel(row.Stat)}  Lv.{lv}   {cost}g";
            if (row.Button != null) row.Button.interactable = Wallet.Gold >= cost;
        }

        private void OnUpgradeClicked(UpgradeRow row)
        {
            if (row == null) return;
            UpgradeStore.TryPurchase(row.Stat);
        }

        private void OnPlayClicked()
        {
            if (_game != null) _game.OnPlayClicked();
        }
    }
}
