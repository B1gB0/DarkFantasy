using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Game.Gameplay;
using _Project.Scripts.Game.GameRoot;
using _Project.Scripts.Game.MainMenu.Root.View;
using _Project.Scripts.Services;
using R3;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using YG;

namespace _Project.Scripts.Game.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        private UIMainMenuRootBinder _uiScene;
        private MainMenuExitParameters _exitParameters;

        private AudioSoundsService _audioSoundsService;

        [Inject]
        private void Construct(AudioSoundsService audioSoundsService)
        {
            _audioSoundsService = audioSoundsService;
        }

        private async void Start()
        {
            await _audioSoundsService.Init();

            _audioSoundsService.PlayMusic(SoundsType.MainMenuMusic);
        }

        public Observable<MainMenuExitParameters> Run(UIRootView uiRoot, MainMenuEnterParameters enterParameters = null)
        {
            _uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(_uiScene.gameObject);

            if (YG2.saves.IsFirstLaunch)
            {
                _uiScene.OnGameplayStarted += GetPrologueParameters;
            }
            else if (!YG2.saves.IsFirstLaunch)
            {
                _uiScene.OnGameplayStarted += GetVillageHubParameters;
            }

            var container = gameObject.scene.GetSceneContainer();
            GameObjectInjector.InjectRecursive(uiRoot.gameObject, container);

            _uiScene.GetUIStateMachineAndStates(uiRoot.UIStateMachine);

            var exitSignalSubject = new Subject<Unit>();
            _uiScene.Bind(exitSignalSubject);

            var exitToGameplaySceneSignal = exitSignalSubject.Select(_ => _exitParameters);

            return exitToGameplaySceneSignal;
        }

        private void GetVillageHubParameters()
        {
            _audioSoundsService.PlayMusic(SoundsType.VillageMusic);

            var sceneName = Scenes.VillageHub;

            var gameplayEnterParameters = new GameplayEnterParameters(sceneName);

            _exitParameters = new MainMenuExitParameters(gameplayEnterParameters);
        }

        private void GetPrologueParameters()
        {
            _audioSoundsService.PlayMusic(SoundsType.ActionMusic);

            var sceneName = Scenes.Prologue;

            var gameplayEnterParameters = new GameplayEnterParameters(sceneName);

            _exitParameters = new MainMenuExitParameters(gameplayEnterParameters);
        }

        private void OnDestroy()
        {
            _uiScene.OnGameplayStarted -= GetVillageHubParameters;
            _uiScene.OnGameplayStarted -= GetPrologueParameters;
        }
    }
}