using Project.Scripts.Game.GameRoot;

namespace Project.Scripts.Game.MainMenu
{
    public class MainMenuExitParameters
    {
        private readonly SceneEnterParameters _targetSceneEnterParameters;

        public MainMenuExitParameters(SceneEnterParameters targetSceneEnterParameters)
        {
            _targetSceneEnterParameters = targetSceneEnterParameters;
        }

        public SceneEnterParameters GetTargetSceneEnterParameters()
        {
            return _targetSceneEnterParameters;
        }
    }
}