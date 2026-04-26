using System;
using System.Collections.Generic;
using _Project.Scripts.Level;
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
        public event Action OnGoToMission;
        
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

            foreach (var missionView in _missionViews)
            {
                missionView.OnMissionChose += OnMoveToMission;
            }
        }

        private void OnDisable()
        {
            _backSceneButton.onClick.RemoveListener(MoveBackToScene);

            foreach (var missionView in _missionViews)
            {
                missionView.OnMissionChose -= OnMoveToMission;
            }
        }
        
        private void OnDestroy()
        {
            transform.DOKill();
        }
        
        public override void Show()
        {
            for (int i = 0; i < _missionService.Missions.Count; i++)
            {
                SetMission(_missionService.Missions[i], _missionViews[i]);
            }
            
            _tweenAnimationService.AnimateScale(transform);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);
        }

        private void MoveBackToScene()
        {
            OnBackToSceneButtonPressed?.Invoke();
        }

        private void OnMoveToMission(Mission mission)
        {
            _missionService.SetCurrentMission(mission.Id);
            OnGoToMission?.Invoke();
        }

        private void SetMission(Mission mission, MissionView missionView)
        {
            missionView.GetMission(mission);
        }
    }
}