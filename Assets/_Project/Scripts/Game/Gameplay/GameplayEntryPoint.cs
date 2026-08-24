using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Game.Gameplay.Root.View;
using _Project.Scripts.Game.GameRoot;
using _Project.Scripts.Game.MainMenu;
using _Project.Scripts.Services;
using _Project.Scripts.UI.Panel;
using _Project.Scripts.UI.StateMachine.States;
using _Project.Scripts.UI.View;
using Cinemachine;
using Cysharp.Threading.Tasks;
using R3;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace _Project.Scripts.Game.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        private const int MinCountValue = 0;
        private const int NextOperationStep = 1;

        [SerializeField] private CinemachineFreeLook _freeLookCamera;
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;
        [SerializeField] private DataFactory _dataFactory;
        [SerializeField] private LevelInitData _levelInitData;
        [SerializeField] private ViewFactory _viewFactory;

        private Level.Level _level;
        private UIRootView _uiRoot;
        private UIGameplayRootBinder _uiScene;
        private Container _container;
        private GameplayExitParameters _exitParameters;

        private IEnemyService _enemyService;
        private IDataBaseService _dataBaseService;
        private IPlayerService _playerService;
        private IFloatingTextService _floatingTextService;
        private ParticleEffectsService _particleEffectsService;
        private AudioSoundsService _audioSoundsService;
        private MissionService _missionService;
        private IUILocalizationService _uiLocalizationService;
        private IPauseService _pauseService;
        private ICurrencyService _currencyService;
        private IInventoryService _inventoryService;

        private EnemyInitData _enemyInitData;
        private PlayerInitData _playerInitData;

        private EndGamePanel _endGamePanel;
        private InventoryPanel _inventoryPanel;

        [Inject]
        private void Construct(
            IEnemyService enemyService,
            IDataBaseService dataBaseService,
            IPlayerService playerService,
            ParticleEffectsService particleEffectsService,
            AudioSoundsService audioSoundsService,
            IFloatingTextService floatingTextService,
            MissionService missionService,
            IUILocalizationService uiLocalizationService,
            IPauseService pauseService,
            ICurrencyService currencyService,
            IInventoryService inventoryService)
        {
            _enemyService = enemyService;
            _dataBaseService = dataBaseService;
            _playerService = playerService;
            _particleEffectsService = particleEffectsService;
            _audioSoundsService = audioSoundsService;
            _floatingTextService = floatingTextService;
            _missionService = missionService;
            _uiLocalizationService = uiLocalizationService;
            _pauseService = pauseService;
            _currencyService = currencyService;
            _inventoryService = inventoryService;
        }

        public async UniTask<Observable<GameplayExitParameters>> Run(
            UIRootView uiRoot,
            GameplayEnterParameters enterParameters = null)
        {
            _container = gameObject.scene.GetSceneContainer();

            _uiRoot = uiRoot;

            await _particleEffectsService.Init();

            _uiScene = Instantiate(_sceneUIRootPrefab);

            _viewFactory.GetEntities(uiRoot, _uiScene, _container, this);

            uiRoot.AttachSceneUI(_uiScene.gameObject);

            GameObjectInjector.InjectRecursive(uiRoot.gameObject, _container);

            _uiScene.GetUIStateMachine(uiRoot.UIStateMachine, _uiRoot.UIRootButtons);

            // uiRoot.ExitPanel.OnExitToMainMenu += GetMainMenuExitParameters;
            //  uiRoot.ExitPanel.OnExitToMainMenu += _uiScene.HandleGoToNextSceneButtonClick;

            await InitData();

            await _dataBaseService.Init();
            await _enemyService.Init();
            await _playerService.Init();
            await _audioSoundsService.Init();
            await _missionService.Init();
            await _uiLocalizationService.Init();
            await _currencyService.Init();

            _playerService.GetSceneObjects(_container, _freeLookCamera);

            _level = FindObjectOfType<Level.Level>();
            GameObjectInjector.InjectObject(_level.gameObject, _container);

            _enemyService.GetData(_enemyInitData);

            _level.GetDependencies(
                _levelInitData,
                _playerInitData,
                _freeLookCamera,
                _viewFactory,
                _uiRoot.UIStateMachine,
                _uiRoot);

            FloatingTextView floatingTextView = await _viewFactory.CreateFloatingTextView();
            floatingTextView.Deactivate();

            _floatingTextService.Init(floatingTextView);

            await _level.OnStartLevel();

            var exitSceneSignalSubject = new Subject<Unit>();
            _uiScene.Bind(exitSceneSignalSubject);

            uiRoot.UIStateMachine.EnterIn<GameplayState>();
            uiRoot.GoldView.Show();
            OnShowJoystickWithAttackButton();

            var scene = SceneManager.GetActiveScene();
            
            _inventoryPanel = await _viewFactory.CreateInventoryPanel();

            _playerService.Player.InputController.OnInventoryButtonPressed += _inventoryPanel.Show;
            _playerService.Player.InputController.OnInventoryButtonPressed += uiRoot.UIRootButtons.Deactivate;
            _playerService.Player.InputController.OnInventoryButtonPressed += _level.HealthBar.Hide;
            _inventoryPanel.OnBackToSceneButtonPressed += _inventoryPanel.Hide;
            _inventoryPanel.OnBackToSceneButtonPressed += uiRoot.UIRootButtons.Activate;
            _inventoryPanel.OnBackToSceneButtonPressed += _level.HealthBar.Show;

            _inventoryService.OnEquippedItem += _uiScene.EquippedItemView.Set;
            _inventoryService.OnUnEquippedItem += _uiScene.EquippedItemView.UnSet;

            if (scene.name != Scenes.VillageHub)
            {
                _endGamePanel = await _viewFactory.CreateEndGamePanel();
                _endGamePanel.GoToVillageButton.onClick.AddListener(GetVillageHubExitParameters);
                _endGamePanel.GoToVillageButton.onClick.AddListener(_uiScene.HandleGoToNextScene);
                _playerService.Player.Health.Die += _endGamePanel.Show;
                _playerService.Player.Health.Die += _endGamePanel.SetDefeatPanel;
                _playerService.Player.Health.Die += _pauseService.OnStopGameWithoutMusic;
                uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged += _endGamePanel.SetLabelText;
                _endGamePanel.OnSpawnPlayer += _level.HealthBar.Show;
            }
            else
            {
                YG2.SaveProgress();
                _currencyService.SaveGold();
            }

            _playerService.Player.Health.Die += _uiScene.ResetCountdownTutorialPointer;
            _playerService.Player.Health.Die += YG2.saves.PlayerCharacteristics.ClearSpeedModifiers;
            _playerService.Player.InputController.OnMoveButtonsPressed += _uiScene.ResetCountdownTutorialPointer;
            uiRoot.SettingsButton.onClick.AddListener(_playerService.Player.InputController.LockPlayerMovement);
            uiRoot.LeaderboardButton.onClick.AddListener(_playerService.Player.InputController.LockPlayerMovement);

            var exitToSceneSignal = exitSceneSignalSubject.Select(_ => _exitParameters);

            _uiScene.ResetCountdownTutorialPointer();
            _uiScene.HandlePCTutorialButtons();

            return exitToSceneSignal;
        }

        private void OnDestroy()
        {
            var scene = SceneManager.GetActiveScene();
            
            _playerService.Player.InputController.OnInventoryButtonPressed -= _inventoryPanel.Show;
            _playerService.Player.InputController.OnInventoryButtonPressed -= _uiRoot.UIRootButtons.Deactivate;
            _playerService.Player.InputController.OnInventoryButtonPressed -= _level.HealthBar.Hide;
            _inventoryPanel.OnBackToSceneButtonPressed -= _inventoryPanel.Hide;
            _inventoryPanel.OnBackToSceneButtonPressed -= _uiRoot.UIRootButtons.Activate;
            _inventoryPanel.OnBackToSceneButtonPressed -= _level.HealthBar.Show;
            
            _inventoryService.OnEquippedItem -= _uiScene.EquippedItemView.Set;
            _inventoryService.OnUnEquippedItem -= _uiScene.EquippedItemView.UnSet;
            
            if (scene.name != Scenes.VillageHub)
            {
                _endGamePanel.GoToVillageButton.onClick.RemoveListener(GetVillageHubExitParameters);
                _endGamePanel.GoToVillageButton.onClick.RemoveListener(_uiScene.HandleGoToNextScene);
                _playerService.Player.Health.Die -= _endGamePanel.Show;
                _playerService.Player.Health.Die -= _endGamePanel.SetDefeatPanel;
                _playerService.Player.Health.Die -= _pauseService.OnStopGameWithoutMusic;
                _uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged -= _endGamePanel.SetLabelText;
                _endGamePanel.OnSpawnPlayer -= _level.HealthBar.Show;
            }

            _playerService.Player.Health.Die -= _uiScene.ResetCountdownTutorialPointer;
            _playerService.Player.Health.Die -= YG2.saves.PlayerCharacteristics.ClearSpeedModifiers;
            _playerService.Player.InputController.OnMoveButtonsPressed -= _uiScene.ResetCountdownTutorialPointer;
            _uiRoot.SettingsButton.onClick.RemoveListener(_playerService.Player.InputController.LockPlayerMovement);
            _uiRoot.LeaderboardButton.onClick.RemoveListener(_playerService.Player.InputController.LockPlayerMovement);
        }

        public void GetGameplayExitParameters()
        {
            _audioSoundsService.PlayMusic(SoundsType.ActionMusic);

            _uiRoot.UIRootButtons.Deactivate();

            int nextNumberLevel = _missionService.CurrentNumberLevel + NextOperationStep;
            _missionService.SetCurrentNumberLevel(nextNumberLevel);

            var sceneName = _missionService.GetSceneNameByNumber(nextNumberLevel);

            var gameplayEnterParameters = new GameplayEnterParameters(sceneName, nextNumberLevel);

            _exitParameters = new GameplayExitParameters(gameplayEnterParameters);
        }

        public void GetVillageHubExitParameters()
        {
            _audioSoundsService.PlayMusic(SoundsType.VillageMusic);

            _missionService.SetCurrentNumberLevel(MinCountValue);

            var sceneName = Scenes.VillageHub;

            var gameplayEnterParameters = new GameplayEnterParameters(sceneName);

            _exitParameters = new GameplayExitParameters(gameplayEnterParameters);
        }

        private void GetMainMenuExitParameters()
        {
            _uiRoot.UIRootButtons.Activate();

            var mainMenuEnterParameters = new MainMenuEnterParameters();
            _exitParameters = new GameplayExitParameters(mainMenuEnterParameters);
        }

        private async UniTask InitData()
        {
            _levelInitData = Instantiate(_levelInitData);
            _enemyInitData = await _dataFactory.CreateSkeletonInitData();
            _playerInitData = await _dataFactory.CreatePlayerInitData();
        }

        private void OnShowJoystickWithAttackButton()
        {
            _playerService.GetButtons(
                _uiScene.Joystick,
                _uiScene.AttackButton,
                _uiScene.RollButton,
                _uiScene.InventoryButton,
                _uiScene.EquippedItemButton);
            
            _uiScene.Joystick.gameObject.SetActive(!YG2.envir.isDesktop);
            _uiScene.AttackButton.gameObject.SetActive(!YG2.envir.isDesktop);
            _uiScene.RollButton.gameObject.SetActive(!YG2.envir.isDesktop);
        }
    }
}