using _Project.Scripts.Services;
using DG.Tweening;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.View
{
    public class GoldView : View
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Transform _showPoint;
        [SerializeField] private Transform _hidePoint;

        private ICurrencyService _currencyService;
        private ITweenAnimationService _tweenAnimationService;

        [Inject]
        private void Construct(ICurrencyService currencyService, ITweenAnimationService tweenAnimationService)
        {
            _currencyService = currencyService;
            _tweenAnimationService = tweenAnimationService;
        }

        private void Start()
        {
            _text.text = _currencyService.Gold.ToString();
            _currencyService.OnGoldValueChanged += SetValue;
        }

        private void OnDestroy()
        {
            _currencyService.OnGoldValueChanged -= SetValue;
            transform.DOKill();
        }

        public override void Show()
        {
            _tweenAnimationService.AnimateMove(transform, _showPoint, _hidePoint);
        }

        private void SetValue(int value)
        {
            _text.text = _currencyService.Gold.ToString();
        }
    }
}