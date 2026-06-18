using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Services;
using _Project.Scripts.UI.Panel;
using _Project.Scripts.UI.StateMachine;
using _Project.Scripts.UI.StateMachine.States;
using _Project.Scripts.UI.View;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Game.GameRoot
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private UISceneContainer _uiSceneContainer;

        [SerializeField] private UIRootButtons _uiRootButtons;
        [SerializeField] private LoadingPanel _loadingPanel;
        [SerializeField] private SettingsPanel _settingsPanel;
        [SerializeField] private LeaderboardPanel _leaderboardPanel;
        [SerializeField] private LocalizationLanguageSwitcher _localizationLanguageSwitcher;
        [SerializeField] private GoldView _goldView;
        
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _leaderboardButton;

        private AudioSoundsService _audioSoundsService;
        private IPauseService _pauseService;

        public UIStateMachine UIStateMachine { get; private set; }
        public UIRootButtons UIRootButtons => _uiRootButtons;
        public Button SettingsButton => _settingsButton;
        public Button LeaderboardButton => _leaderboardButton;
        public GoldView GoldView => _goldView;
        public LocalizationLanguageSwitcher LocalizationLanguageSwitcher => _localizationLanguageSwitcher;

        [Inject]
        private void Construct(AudioSoundsService audioSoundsService, IPauseService pauseService)
        {
            _audioSoundsService = audioSoundsService;
            _pauseService = pauseService;
        }

        private void Awake()
        {
            UIStateMachine = new UIStateMachine();
            UIStateMachine.AddState(new LeaderboardPanelState(_leaderboardPanel));
            UIStateMachine.AddState(new LoadingPanelState(_loadingPanel));
            UIStateMachine.AddState(new SettingsPanelState(_settingsPanel));
        }

        private void OnEnable()
        {
            _settingsPanel.OnBackToSceneButtonPressed += ShowUIScene;
            _settingsButton.onClick.AddListener(StopGame);
            _settingsButton.onClick.AddListener(_settingsPanel.Show);
            _settingsPanel.OnBackToSceneButtonPressed += PlayGame;
            
            _leaderboardButton.onClick.AddListener(ShowLeaderboardPanel);
            _leaderboardPanel.OnBackToSceneButtonPressed += ShowUIScene;
            _leaderboardPanel.OnBackToSceneButtonPressed += PlayGame;
        }

        private void OnDisable()
        {
            _settingsPanel.OnBackToSceneButtonPressed -= ShowUIScene;
            _settingsButton.onClick.RemoveListener(StopGame);
            _settingsButton.onClick.RemoveListener(_settingsPanel.Show);
            _settingsPanel.OnBackToSceneButtonPressed -= PlayGame;
            
            _leaderboardButton.onClick.RemoveListener(ShowLeaderboardPanel);
            _leaderboardPanel.OnBackToSceneButtonPressed -= ShowUIScene;
            _leaderboardPanel.OnBackToSceneButtonPressed -= PlayGame;
        }

        public void ShowLoadingProgress(float progress)
        {
            _loadingPanel.SetProgressText(progress);
        }

        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();

            sceneUI.transform.SetParent(_uiSceneContainer.transform, false);
        }
        
        private void ShowLeaderboardPanel()
        {
            _audioSoundsService.PauseAllSounds();
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();
            UIStateMachine.EnterIn<LeaderboardPanelState>();
            StopGame();
        }
        
        private void PlayGame()
        {
            if (SceneManager.GetActiveScene().name == Scenes.MainMenu)
                return;

            _pauseService.OnPlayGame();
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();
            _audioSoundsService.ResumeAllSounds();
        }

        private void StopGame()
        {
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();
            
            if (SceneManager.GetActiveScene().name == Scenes.MainMenu)
                return;

            _pauseService.OnStopGameWithoutMusic();
        }

        private void ShowUIScene()
        {
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();

            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == Scenes.MainMenu)
                UIStateMachine.EnterIn<MainMenuState>();

            if (sceneName != Scenes.MainMenu)
                UIStateMachine.EnterIn<GameplayState>();
        }

        private void ClearSceneUI()
        {
            var childCount = _uiSceneContainer.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Destroy(_uiSceneContainer.transform.GetChild(i).gameObject);
            }
        }
    }
}