using _Project.Scripts.Services;
using DG.Tweening;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public abstract class Bar : View
    {
        [SerializeField] protected Slider SmoothSlider;
        [SerializeField] protected Slider Slider;

        [SerializeField] protected TMP_Text TextValue;

        [SerializeField] private bool _isShowTextValue;

        [SerializeField] private Transform _showPoint;
        [SerializeField] private Transform _hidePoint;
        [SerializeField] private Transform _weaponPanelPoint;

        private ITweenAnimationService _tweenAnimationService;

        [Inject]
        private void Construct(ITweenAnimationService tweenAnimationService)
        {
            _tweenAnimationService = tweenAnimationService;
        }
        
        private void Start()
        {
            TextValue.gameObject.SetActive(_isShowTextValue);
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

        public void MoveToWeaponPanelPosition()
        {
            _tweenAnimationService.AnimateMove(transform, _weaponPanelPoint, _showPoint);
        }

        public void MoveToShowPosition()
        {
            _tweenAnimationService.AnimateMove(transform, _showPoint, _weaponPanelPoint);
        }

        public void GetPoints(Transform showPoint, Transform hidePoint, Transform weaponPanelPoint)
        {
            _showPoint = showPoint;
            _hidePoint = hidePoint;
            _weaponPanelPoint = weaponPanelPoint;
        }

        protected void SetValues(float currentValue, float maxValue, float targetValue)
        {
            SmoothSlider.value = currentValue / maxValue;

            Slider.value = targetValue / maxValue;

            if (!_isShowTextValue) return;
            TextValue.text = (int)targetValue + "/" + (int)maxValue;
        }
    }
}