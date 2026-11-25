using DG.Tweening;
using Project.Scripts.Services;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.View
{
    public abstract class Bar : View
    {
        [SerializeField] private Slider _smoothSlider;
        [SerializeField] private Slider _slider;

        [SerializeField] private TMP_Text _textValue;

        [SerializeField] private bool _isShowTextValue;

        [SerializeField] private Transform _showPoint;
        [SerializeField] private Transform _hidePoint;

        private ITweenAnimationService _tweenAnimationService;

        [Inject]
        private void Construct(ITweenAnimationService tweenAnimationService)
        {
            _tweenAnimationService = tweenAnimationService;
        }

        private void Start()
        {
            _textValue.gameObject.SetActive(_isShowTextValue);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public override void Show()
        {
            _tweenAnimationService.AnimateMove(transform, _showPoint, _hidePoint);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateMove(transform, _showPoint, _hidePoint, true);
        }

        public void GetPoints(Transform showPoint, Transform hidePoint)
        {
            _showPoint = showPoint;
            _hidePoint = hidePoint;
        }

        protected void SetValues(float currentValue, float maxValue, float targetValue)
        {
            _smoothSlider.value = currentValue / maxValue;

            _slider.value = targetValue / maxValue;

            if (!_isShowTextValue) return;
            _textValue.text = (int)targetValue + "/" + (int)maxValue;
        }
    }
}