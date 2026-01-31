using _Project.Scripts.Game.GameRoot;

namespace _Project.Scripts.Game.Gameplay
{
    public class GameplayExitParameters
    {
        public readonly SceneEnterParameters TargetSceneEnterParameters;

        public GameplayExitParameters(SceneEnterParameters targetSceneEnterParameters)
        {
            TargetSceneEnterParameters = targetSceneEnterParameters;
        }
    }
}