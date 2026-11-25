using Project.Scripts.Game.GameRoot;

namespace Project.Scripts.Game.Gameplay
{
    public class GameplayExitParameters
    {
        private readonly SceneEnterParameters _targetSceneEnterParameters;

        public GameplayExitParameters(SceneEnterParameters targetSceneEnterParameters)
        {
            _targetSceneEnterParameters = targetSceneEnterParameters;
        }

        public SceneEnterParameters GetTargetSceneEnterParameters()
        {
            return _targetSceneEnterParameters;
        }
    }
}