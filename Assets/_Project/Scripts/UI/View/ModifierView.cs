using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class ModifierView : MonoBehaviour
    {
        [SerializeField] private RadialBar _radialBar;
        [SerializeField] private Image _icon;
        
        private Sprite _lastIcon;
        private Color _lastColor;
        private float _lastProgress = float.NaN;
        private bool _initialized;

        public void SetIcon(Sprite icon)
        {
            if (_initialized && _lastIcon == icon) return;
            
            _lastIcon = icon;
            _icon.sprite = icon;
            _initialized = true;
        }

        public void SetColor(Color color)
        {
            if (_initialized && _lastColor == color) return;

            _lastColor = color;
            
            _radialBar.SetColor(color);
        }

        public void SetProgress(float remainingTime, float totalDuration)
        {
            float normalized = totalDuration <= 0f
                ? 0f
                : Mathf.Clamp01(remainingTime / totalDuration);
            
            if (_initialized && Mathf.Approximately(_lastProgress, normalized)) return;

            _lastProgress = normalized;

            _radialBar.SetProgressImmediate(normalized);
        }

        public void Reset()
        {
            _initialized = false;
            _lastIcon = null;
            _lastColor = default;
            _lastProgress = float.NaN;

            _icon.sprite = null;
            _radialBar.SetProgressImmediate(1f);
        }
    }
}