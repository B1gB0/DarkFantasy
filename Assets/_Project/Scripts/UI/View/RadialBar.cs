using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class RadialBar : View
    {
        private const float RecoveryRate = 10f;
        private const float ApproximateValue = 0.01f;

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
        
        public void OnChangeValue(float currentValue, float targetValue, float maxValue)
        {
            CancelAnimation();
            _cancellationTokenSource = new CancellationTokenSource();
            SetValueAsync(currentValue, targetValue, maxValue, _cancellationTokenSource.Token).Forget();
        }

        public void SetColor(Color color)
        {
            _barInstance.SetColor(_color, color);
        }

        private async UniTask SetValueAsync(
            float currentValue,
            float targetValue,
            float maxValue,
            CancellationToken token)
        {
            if (maxValue <= 0f)
                return;

            while (!token.IsCancellationRequested &&
                   Mathf.Abs(currentValue - targetValue) > ApproximateValue)
            {
                currentValue = Mathf.MoveTowards(
                    currentValue,
                    targetValue,
                    RecoveryRate * Time.unscaledDeltaTime);

                _barInstance?.SetFloat(_removedSegments, currentValue / maxValue);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            if (!token.IsCancellationRequested)
                _barInstance?.SetFloat(_removedSegments, targetValue / maxValue);
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