using UnityEngine;
using UnityEngine.UI;

namespace Tower
{
    [DisallowMultipleComponent]
    public sealed class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private Game _game;
        [SerializeField] private Button _homeButton;

        private void Awake()
        {
            if (_homeButton != null) _homeButton.onClick.AddListener(OnHomeClicked);
        }

        private void OnHomeClicked()
        {
            if (_game != null) _game.OnHomeClicked();
        }
    }
}
