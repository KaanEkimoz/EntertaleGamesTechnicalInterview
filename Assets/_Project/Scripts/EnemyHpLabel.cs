using UnityEngine;

namespace Tower
{
    public sealed class EnemyHpLabel : MonoBehaviour
    {
        [SerializeField] private Enemy _enemy;
        [SerializeField] private TextMesh _text;

        private Health _bound;

        private void OnEnable() => StartCoroutine(BindRoutine());

        private System.Collections.IEnumerator BindRoutine()
        {
            // Wait one frame so Enemy.Init() runs first.
            while (_enemy != null && _enemy.Logic == null) yield return null;
            if (_enemy == null || _enemy.Logic == null) yield break;
            _bound = _enemy.Logic;
            _bound.OnHealthChanged += HandleChanged;
            Refresh();
        }

        private void OnDisable()
        {
            if (_bound != null) _bound.OnHealthChanged -= HandleChanged;
            _bound = null;
        }

        private void HandleChanged(float _) => Refresh();

        private void Refresh()
        {
            if (_text == null || _bound == null) return;
            _text.text = $"{_bound.Current:F0}/{_bound.Max:F0}";
        }
    }
}
