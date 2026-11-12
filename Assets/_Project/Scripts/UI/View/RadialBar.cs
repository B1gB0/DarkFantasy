using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class RadialBar : View
    {
        private const int DefaultBarValue = 0;
        private const int DefaultBackgroundBarValue = 1;

        private readonly int _removedSegments = Shader.PropertyToID("_RemovedSegments");
        private readonly int _color = Shader.PropertyToID("_Color");

        [SerializeField] private Image _barImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Material _barMaterial;
        [SerializeField] private Material _backgroundBarMaterial;

        private Material _barInstance;
        private Material _backgroundInstance;

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            if (_barImage != null && _barMaterial != null)
            {
                _barInstance = new Material(_barMaterial);
                _barImage.material = _barInstance;
            }

            if (_backgroundImage != null && _backgroundBarMaterial != null)
            {
                _backgroundInstance = new Material(_backgroundBarMaterial);
                _backgroundImage.material = _backgroundInstance;
            }
        }

        private void Start()
        {
            _barInstance?.SetFloat(_removedSegments, DefaultBarValue);
            _backgroundInstance?.SetFloat(_removedSegments, DefaultBackgroundBarValue);
        }

        private void OnDestroy()
        {
            CancelAnimation();

            if (_barInstance != null) Destroy(_barInstance);
            if (_backgroundInstance != null) Destroy(_backgroundInstance);
        }
        
        public void SetProgressImmediate(float normalized01)
        {
            CancelAnimation();
            _barInstance?.SetFloat(_removedSegments, Mathf.Clamp01(normalized01));
        }

        public void SetColor(Color color)
        {
            _barInstance.SetColor(_color, color);
        }

        private void CancelAnimation()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }
}