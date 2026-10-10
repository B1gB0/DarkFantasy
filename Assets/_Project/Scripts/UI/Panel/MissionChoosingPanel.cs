using System;
using System.Collections.Generic;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Level;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using DG.Tweening;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.Panel
{
    public class MissionChoosingPanel : View.View
    {
        private const string PrologueId = "prologue";
        private const int StepDifficulty = 1;

        private static readonly int DifficultyCount = Enum.GetValues(typeof(LevelDifficulty)).Length;

        [SerializeField] private Button _backSceneButton;
        [SerializeField] private List<NewMissionView> _missionViews;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;
        [SerializeField] private TMP_Text _difficultyText;
        [SerializeField] private Image _locker;

        private ITweenAnimationService _tweenAnimationService;
        private MissionService _missionService;
        private ICurrencyService _currencyService;
        private IProgressionService _progressionService;
        private IUILocalizationService _uiLocalizationService;

        private int _difficultyIndex;

        public event Action OnBackToSceneButtonPressed;
        public event Action OnGoToMission;

        [Inject]
        private void Construct(
            ITweenAnimationService tweenAnimationService,
            MissionService missionService,
            ICurrencyService currencyService,
            IProgressionService progressionService,
            IUILocalizationService uiLocalizationService)
        {
            _tweenAnimationService = tweenAnimationService;
            _missionService = missionService;
            _currencyService = currencyService;
            _progressionService = progressionService;
            _uiLocalizationService = uiLocalizationService;
        }

        private void Start()
        {
            Deactivate();
        }

        private void OnEnable()
        {
            _backSceneButton.onClick.AddListener(MoveBackToScene);
            _nextButton.onClick.AddListener(NextDifficulty);
            _previousButton.onClick.AddListener(PreviousDifficulty);

            foreach (var missionView in _missionViews)
            {
                missionView.OnMissionChose += OnMoveToMission;
            }
        }

        private void OnDisable()
        {
            _backSceneButton.onClick.RemoveListener(MoveBackToScene);
            _nextButton.onClick.RemoveListener(NextDifficulty);
            _previousButton.onClick.RemoveListener(PreviousDifficulty);

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
                if (_missionService.Missions[i].Id == PrologueId)
                    continue;

                SetMission(_missionService.Missions[i], _missionViews[i]);

                if (YG2.saves.IsBanditCampUnlock)
                {
                    _missionViews[1].gameObject.SetActive(true);
                }

                if (YG2.saves.IsCastleUnlock)
                {
                    _missionViews[2].gameObject.SetActive(true);
                }
            }

            _difficultyIndex = (int)_progressionService.CurrentDifficulty;
            
            UpdateDifficultyDisplay();

            _tweenAnimationService.AnimateScale(transform);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);
        }

        private void NextDifficulty()
        {
            _difficultyIndex = (_difficultyIndex + StepDifficulty) % DifficultyCount;
            UpdateDifficultyDisplay();
        }

        private void PreviousDifficulty()
        {
            _difficultyIndex = (_difficultyIndex - StepDifficulty + DifficultyCount) % DifficultyCount;
            UpdateDifficultyDisplay();
        }

        private void UpdateDifficultyDisplay()
        {
            var difficulty = (LevelDifficulty)_difficultyIndex;
            bool unlocked = _progressionService.IsDifficultyUnlocked(difficulty);

            _locker.gameObject.SetActive(!unlocked);
            _difficultyText.gameObject.SetActive(unlocked);

            if (!unlocked)
            {
                return;
            }

            var textType = GetDifficultyTextType(difficulty);
            _difficultyText.text = _uiLocalizationService.GetLocalizedText(textType);

            _difficultyText.color = GetDifficultyColor(difficulty);

            if (_progressionService.CurrentDifficulty != difficulty)
                _progressionService.SetDifficulty(difficulty);
        }

        private UITextType GetDifficultyTextType(LevelDifficulty difficulty)
        {
            return difficulty switch
            {
                LevelDifficulty.Low => UITextType.DifficultyLow,
                LevelDifficulty.Medium => UITextType.DifficultyMedium,
                LevelDifficulty.High => UITextType.DifficultyHigh,
                _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
            };
        }

        private Color GetDifficultyColor(LevelDifficulty difficulty)
        {
            return difficulty switch
            {
                LevelDifficulty.Low => Colors.GetColor(ColorName.LowColor),
                LevelDifficulty.Medium => Colors.GetColor(ColorName.MediumColor),
                LevelDifficulty.High => Colors.GetColor(ColorName.HighColor),
                _ => Colors.GetColor(ColorName.DefaultWhiteTextColor),
            };
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

        private void SetMission(Mission mission, NewMissionView missionView)
        {
            missionView.GetMission(mission);
        }
    }
}