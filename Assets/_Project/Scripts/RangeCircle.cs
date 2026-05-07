using UnityEngine;

namespace Tower
{
    [RequireComponent(typeof(LineRenderer))]
    [ExecuteAlways]
    public sealed class RangeCircle : MonoBehaviour
    {
        [SerializeField] private float _radius = 8f;
        [SerializeField, Range(8, 256)] private int _segments = 64;
        [SerializeField] private float _width = 0.08f;
        [SerializeField] private Color _color = new Color(0.40f, 0.85f, 1f, 0.85f);

        private LineRenderer _line;

        public void SetRadius(float r) { _radius = r; Rebuild(); }

        private void OnEnable()
        {
            EnsureLine();
            Rebuild();
        }

        private void EnsureLine()
        {
            _line = GetComponent<LineRenderer>();
            if (_line.material == null || _line.material.shader == null)
                _line.material = new Material(Shader.Find("Sprites/Default"));
            _line.useWorldSpace = false;
            _line.loop = true;
            _line.startWidth = _width;
            _line.endWidth = _width;
            _line.startColor = _color;
            _line.endColor = _color;
            _line.numCornerVertices = 2;
            _line.numCapVertices = 0;
            _line.sortingOrder = 4;
        }

        private void Rebuild()
        {
            if (_line == null) _line = GetComponent<LineRenderer>();
            _line.positionCount = _segments;
            for (int i = 0; i < _segments; i++)
            {
                float t = (float)i / _segments * Mathf.PI * 2f;
                _line.SetPosition(i, new Vector3(Mathf.Cos(t) * _radius, Mathf.Sin(t) * _radius, 0f));
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!isActiveAndEnabled) return;
            EnsureLine();
            Rebuild();
        }
#endif
    }
}
