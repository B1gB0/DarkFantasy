using Cysharp.Threading.Tasks;
using Project.Scripts.Game.GameRoot;
using R3;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        // [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
        // [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;
        
        private UIRootView _uiRoot;
        private Container _container;
        private GameplayExitParameters _exitParameters;
        
        public async UniTask<Observable<GameplayExitParameters>> Run(
            UIRootView uiRoot,
            GameplayEnterParameters enterParameters)
        {
            // uiRoot.ExitButton.gameObject.SetActive(true);

            _container = gameObject.scene.GetSceneContainer();

            _uiRoot = uiRoot;

//             // await InitData();
//
//             await _particleEffectsService.Init();
//
//             _uiScene = Instantiate(_sceneUIRootPrefab);
//
//             _viewFactory.GetUIRootAndUIScene(uiRoot, _uiScene, _container);
//
//             FloatingTextView textView = await _viewFactory.CreateDamageTextView();
//             textView.Hide();
//             _floatingTextService.Init(textView);
//
//             _goldView = await _viewFactory.CreateGoldView();
//
//             _experiencePoints = new ExperiencePoints(_playerService);
//
// #if UNITY_EDITOR
//             _cheatPanel = await _viewFactory.CreateCheatPanel(_experiencePoints);
// #endif
//
//             InitEcs();
//
//             _healthBar = await _viewFactory.CreateHealthBar(_gameInitSystem.PlayerHealth);
//             _progressBar = await _viewFactory.CreateProgressBar(_experiencePoints, _gameInitSystem.PlayerTransform);
//
//             _levelUpPanel = await _viewFactory.CreateLevelUpPanel();
//             _endGamePanel = await _viewFactory.CreateEndGamePanel();
//             _endGamePanel.GetServices(_experiencePoints, _uiScene.WeaponPanel);
//
//             _weaponFactory.GetData(_gameInitSystem.PlayerTransform, _weaponHolder, _uiScene.WeaponPanel);
//             await _weaponFactory.CreateEnemyDetectorForPlayer();
//
//             _levelUpPanel.GetServices(_weaponFactory, _weaponHolder, _uiScene.WeaponPanel, _healthBar);
//             _weaponFactory.GetMinesButton(_uiScene.MinesButton);
//
//             uiRoot.AttachSceneUI(_uiScene.gameObject);
//
//             _uiScene.GetUIStateMachine(uiRoot.UIStateMachine, uiRoot.UIRootButtons);
//
//             _weaponFactory.WeaponIsCreated += _uiScene.WeaponPanel.SetData;
//
//             _goldView.Show();
//
//             _playerService.GetJoystick(_uiScene.Joystick);
//
//             _gameInitSystem.PlayerHealth.Die += _endGamePanel.Show;
//             _gameInitSystem.PlayerHealth.Die += _pauseService.OnStopGameWithMusic;
//             _gameInitSystem.PlayerHealth.Die += _uiScene.ResetCountdownTutorialPointer;
//             _gameInitSystem.PlayerHealth.Die += _uiRoot.UIRootButtons.Hide;
//             _gameInitSystem.PlayerHealth.Die += _endGamePanel.SetDefeatPanel;
//             _gameInitSystem.PlayerHealth.Die += _progressBar.Hide;
//             _gameInitSystem.PlayerHealth.IsSpawnedHealingText += _floatingTextService.OnChangedFloatingText;
//
//             uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged += _progressBar.ChangeText;
//             uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged += _levelUpPanel.SetTitle;
//             uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged += _endGamePanel.SetLabelText;
//
//             _level.EndLevelTrigger.IsLevelCompleted += _levelUpPanel.OnEndGameTriggerIsReached;
//             _level.EndLevelTrigger.IsLevelCompleted += _uiRoot.UIRootButtons.Hide;
//             _level.EndLevelTrigger.IsLevelCompleted += _endGamePanel.SetVictoryPanel;
//
//             _levelUpPanel.OnContinueButtonIsClicked += _endGamePanel.Show;
//             _levelUpPanel.OnContinueButtonIsClicked += _pauseService.OnStopGameWithMusic;
//
//             _gameInitSystem.PlayerIsSpawned += _uiScene.WeaponPanel.Show;
//             _gameInitSystem.PlayerIsSpawned += _progressBar.Show;
//             _gameInitSystem.PlayerIsSpawned += _healthBar.Show;
//             _gameInitSystem.PlayerIsSpawned += OnShowJoystick;
//
//             _endGamePanel.OnSpawnPlayer += _gameInitSystem.CreateCapsule;
//             _endGamePanel.OnRewardAdSuccessShowed += uiRoot.UIRootButtons.Show;
//
//             _endGamePanel.GoToMainMenuButton.onClick.AddListener(GetMainMenuExitParameters);
//             _endGamePanel.GoToMainMenuButton.onClick.AddListener(_uiScene.HandleGoToNextSceneButtonClick);
//
//             _endGamePanel.NextLevelButton.onClick.AddListener(GetGameplayExitParameters);
//             _endGamePanel.NextLevelButton.onClick.AddListener(_uiScene.HandleGoToNextSceneButtonClick);
//
//             uiRoot.ExitPanel.OnExitToMainMenu += GetMainMenuExitParameters;
//             uiRoot.ExitPanel.OnExitToMainMenu += _uiScene.HandleGoToNextSceneButtonClick;
//
//             _playerService.PlayerActor.PlayerInputController.OnMoveButtonsPressed +=
//                 _uiScene.ResetCountdownTutorialPointer;
//
// #if UNITY_EDITOR
//             _uiScene.CheatsButton.onClick.AddListener(_cheatPanel.Show);
// #endif
//
//             _weaponFactory.MinesIsCreated += _uiScene.ShowMinesButton;
//             _experiencePoints.CurrentLevelIsUpgraded += _levelUpPanel.OnCurrentLevelIsUpgraded;
//
            var exitSceneSignalSubject = new Subject<Unit>();
//             _uiScene.Bind(exitSceneSignalSubject);
//
            var exitToSceneSignal = exitSceneSignalSubject.Select(_ => _exitParameters);
//
//             await _level.OnStartLevel();
//             await TryLoadWeapons();
//
//             _uiScene.ResetCountdownTutorialPointer();

            return exitToSceneSignal;
        }
    }
}