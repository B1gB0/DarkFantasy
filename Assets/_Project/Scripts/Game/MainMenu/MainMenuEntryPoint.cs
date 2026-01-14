using Project.Scripts.Game.Gameplay;
using Project.Scripts.Game.GameRoot;
using Project.Scripts.Game.MainMenu.Root.View;
using R3;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using YG;

namespace Project.Scripts.Game.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        private UIMainMenuRootBinder _uiScene;
        private MainMenuExitParameters _exitParameters;
        
        public Observable<MainMenuExitParameters> Run(UIRootView uiRoot, MainMenuEnterParameters enterParameters)
        {
            // uiRoot.ExitButton.gameObject.SetActive(false);

            _uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(_uiScene.gameObject);

            _uiScene.OnGameplayStarted += GetMainMenuExitParameters;

            var container = gameObject.scene.GetSceneContainer();
            GameObjectInjector.InjectRecursive(uiRoot.gameObject, container);

            _uiScene.GetUIStateMachineAndStates(uiRoot.UIStateMachine);

            var exitSignalSubject = new Subject<Unit>();
            _uiScene.Bind(exitSignalSubject);

            var exitToGameplaySceneSignal = exitSignalSubject.Select(_ => _exitParameters);

            return exitToGameplaySceneSignal;
        }
        
        private void GetMainMenuExitParameters()
        {
            var sceneName = Scenes.VillageHub;

            var gameplayEnterParameters = new GameplayEnterParameters(sceneName);

            _exitParameters = new MainMenuExitParameters(gameplayEnterParameters);
        }
    }
}
