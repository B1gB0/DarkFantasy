using System;
using System.Collections.Generic;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Panel
{
    public class MissionChoosingPanel : View.View
    {
        [SerializeField] private Button _backSceneButton;
        [SerializeField] private List<MissionView> _missionViews;

        private ITweenAnimationService _tweenAnimationService;
        private MissionService _missionService;
        private ICurrencyService _currencyService;
        
        public event Action OnBackToSceneButtonPressed;
        
        [Inject]
        private void Construct(
            ITweenAnimationService tweenAnimationService,
            MissionService missionService,
            ICurrencyService currencyService)
        {
            _tweenAnimationService = tweenAnimationService;
            _missionService = missionService;
            _currencyService = currencyService;
        }
        
        private void Start()
        {
            Deactivate();
        }

        private void OnEnable()
        {
            _backSceneButton.onClick.AddListener(MoveBackToScene);
        }

        private void OnDisable()
        {
            _backSceneButton.onClick.RemoveListener(MoveBackToScene);
        }
        
        private void OnDestroy()
        {
            transform.DOKill();
        }
        
        public override void Show()
        {
            SetMissionViews();
            _tweenAnimationService.AnimateScale(transform);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);
        }
        
        public void OnChangeLanguage()
        {
            SetMissionViews();
        }
        
        private void MoveBackToScene()
        {
            OnBackToSceneButtonPressed?.Invoke();
        }

        private void SetMissionViews()
        {
            
        }
    }
}