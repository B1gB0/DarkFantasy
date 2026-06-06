using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Experience;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Services;
using DG.Tweening;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.Panel
{
    public class EndGamePanel : MonoBehaviour
    {
        private const int CountCorrectFactor = 1;
        private const string RewardAdRebornId = "RebornPlayer";

        [SerializeField] private TMP_Text _labelText;
        [SerializeField] private TMP_Text _accumulatedKillsText;
        [SerializeField] private TMP_Text _accumulatedGoldText;
        [SerializeField] private TMP_Text _accumulatedScoreText;

        [SerializeField] private Button _goToVillageButton;
        [SerializeField] private Button _rebornPlayerButton;
        // [SerializeField] private Button _nextLevelButton;

        [SerializeField] private List<Image> _images;
        [SerializeField] private GameObject _rootWindow;

        private IPauseService _pauseService;
        private MissionService _missionService;
        private ICurrencyService _currencyService;
        private IUILocalizationService _uiLocalizationService;
        private ITweenAnimationService _tweenAnimationService;
        private IPlayerService _playerService;
        private IExperiencePoints _experiencePoints;
        
        // private WeaponPanel _weaponPanel;
        private UILocalizationData _uiLocalizationData;

        public event Action OnRewardAdSuccessShowed;
        public event Action OnSpawnPlayer;

        public Button GoToVillageButton => _goToVillageButton;
        // public Button NextLevelButton => _nextLevelButton;

        [Inject]
        public void Construct(
            IPauseService pauseService,
            MissionService missionService,
            ICurrencyService currencyService,
            IUILocalizationService uiLocalizationService,
            ITweenAnimationService tweenAnimationService,
            IPlayerService playerService,
            IExperiencePoints experiencePoints)
        {
            _pauseService = pauseService;
            _missionService = missionService;
            _currencyService = currencyService;
            _uiLocalizationService = uiLocalizationService;
            _tweenAnimationService = tweenAnimationService;
            _playerService = playerService;
            _experiencePoints = experiencePoints;
        }

        private void OnEnable()
        {
            _goToVillageButton.onClick.AddListener(Hide);
            _rebornPlayerButton.onClick.AddListener(OnShowRewardAd);

#if UNITY_EDITOR
            _rebornPlayerButton.onClick.AddListener(Hide);
            _rebornPlayerButton.onClick.AddListener(OnReborn);
#endif
        }

        private void Start()
        {
            OnRewardAdSuccessShowed += OnRewardSuccess;
            
            YG2.onShowWindowGame -= _pauseService.OnPlayGame;
            YG2.onHideWindowGame -= _pauseService.OnStopGameWithMusic;
            
            // _nextLevelButton.onClick.AddListener(_pauseService.OnPlayGame);
            _goToVillageButton.onClick.AddListener(_pauseService.OnPlayGame);
            
#if UNITY_EDITOR
            _rebornPlayerButton.onClick.AddListener(_pauseService.OnPlayGame);
#endif
        }

        private void OnDisable()
        {
            _goToVillageButton.onClick.RemoveListener(Hide);
            _rebornPlayerButton.onClick.RemoveListener(OnShowRewardAd);

#if UNITY_EDITOR
            _rebornPlayerButton.onClick.RemoveListener(Hide);
            _rebornPlayerButton.onClick.RemoveListener(OnReborn);
#endif
        }

        private void OnDestroy()
        {
            YG2.onShowWindowGame += _pauseService.OnPlayGame;
            YG2.onHideWindowGame += _pauseService.OnStopGameWithMusic;
            
            _experiencePoints.ResetAccumulatedValues();
            _currencyService.ResetAccumulatedGold();
            
            // _nextLevelButton.onClick.RemoveListener(_pauseService.OnPlayGame);
            _goToVillageButton.onClick.RemoveListener(_pauseService.OnPlayGame);
            
#if UNITY_EDITOR
            _rebornPlayerButton.onClick.RemoveListener(_pauseService.OnPlayGame);
#endif
            
            OnRewardAdSuccessShowed -= OnRewardSuccess;
            _rootWindow.transform.DOKill();
        }

        public void SetVictoryPanel()
        {
            if (_missionService.CurrentNumberLevel ==
                _missionService.CurrentMission.Maps.Count - CountCorrectFactor)
            {
                // _nextLevelButton.gameObject.SetActive(false);

                switch (_missionService.CurrentMission.Id)
                {
                    case Missions.Graveyard:
                        YG2.saves.IsBanditVillageUnlock = true;
                        YG2.SaveProgress();
                        break;
                    case Missions.BanditVillage:
                        YG2.saves.IsCastleUnlock = true;
                        YG2.SaveProgress();
                        break;
                }
            }
            else
            {
                // _nextLevelButton.gameObject.SetActive(true);
            }

            _rebornPlayerButton.gameObject.SetActive(false);

            SetLocalizationData(UITextType.VictoryPanelTitle);

            OnChangeColor(Colors.GetColor(ColorName.BlueUIPanelColor));
        }

        public void SetDefeatPanel()
        {
            _rebornPlayerButton.gameObject.SetActive(true);
            // _nextLevelButton.gameObject.SetActive(false);

            SetLocalizationData(UITextType.DefeatPanelTitle);

            OnChangeColor(Colors.GetColor(ColorName.RedUIPanelColor));
        }

        public void Show()
        {
            _accumulatedGoldText.text = _currencyService.AccumulatedGold.ToString();
            _accumulatedKillsText.text = _experiencePoints.AccumulatedKills.ToString();
            _accumulatedScoreText.text = _experiencePoints.AccumulatedScore.ToString();
            
            // _weaponPanel.Hide();
            _tweenAnimationService.AnimateScale(transform);
        }

        public void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);
            // _weaponPanel.Show();
        }

        public void SetLabelText()
        {
            if (_uiLocalizationData == null)
                return;

            _labelText.text = YG2.lang switch
            {
                LocalizationCode.Ru => _uiLocalizationData.NameRu,
                LocalizationCode.En => _uiLocalizationData.NameEn,
                LocalizationCode.Tr => _uiLocalizationData.NameTr,
                _ => _labelText.text
            };
        }

        public void GetServices(ExperiencePoints experiencePoints)
        {
            _experiencePoints = experiencePoints;
            // _weaponPanel = weaponPanel;
        }

        private void SetLocalizationData(UITextType type)
        {
            _uiLocalizationData = _uiLocalizationService.GetLevelTextData(type);

            SetLabelText();
        }

        private void OnChangeColor(Color color)
        {
            foreach (var image in _images)
            {
                image.color = color;
            }
        }

#if UNITY_EDITOR
        private void OnReborn()
        {
            _playerService.SpawnPlayer();
            OnRewardAdSuccessShowed?.Invoke();
        }
#endif

        private void OnRewardSuccess()
        {
            Hide();
            _pauseService.OnPlayGame();
            _pauseService.EnableEventSystem();
            _playerService.SpawnPlayer();
            OnSpawnPlayer?.Invoke();
        }

        private void OnShowRewardAd()
        {
            _pauseService.DisableEventSystem();
            YG2.RewardedAdvShow(RewardAdRebornId, OnRewardAdSuccessShowed);
        }
    }
}