using System;
using _Project.Scripts.Services;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.Panel
{
    public class LeaderboardPanel : View.View
    {
        [SerializeField] private LeaderboardYG _leaderboardYg;
        [SerializeField] private Button _leaderboardButton;
        [SerializeField] private Button _backToSceneButton;
        
        private ITweenAnimationService _tweenAnimationService;

        public event Action OnBackToSceneButtonPressed;
        
        [Inject]
        private void Construct(ITweenAnimationService tweenAnimationService)
        {
            _tweenAnimationService = tweenAnimationService;
        }

        private void OnEnable()
        {
            _leaderboardYg.SetLeaderboard(YG2.saves.AcumulatedScore);
            _leaderboardYg.UpdateLB();
            
            _backToSceneButton.onClick.AddListener(MoveBackToScene);
            _leaderboardButton.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _backToSceneButton.onClick.RemoveListener(MoveBackToScene);
            _leaderboardButton.gameObject.SetActive(true);
        }

        public override void Show()
        {
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
    }
}