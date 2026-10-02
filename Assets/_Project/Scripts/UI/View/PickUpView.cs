using System;
using _Project.Scripts.Services;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class PickUpView : View
    {
        [SerializeField] private Button _mobileButton;
        [SerializeField] private GameObject _desktopButton;
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
            if (YG2.envir.isDesktop)
            {
                _mobileButton.gameObject.SetActive(false);
                _desktopButton.gameObject.SetActive(true);
            }
            else
            {
                _mobileButton.gameObject.SetActive(true);
                _desktopButton.gameObject.SetActive(false);
            }
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
    }
}