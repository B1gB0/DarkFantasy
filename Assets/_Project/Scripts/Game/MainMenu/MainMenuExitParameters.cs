using _Project.Scripts.Game.GameRoot;

namespace _Project.Scripts.Game.MainMenu
{
    public class MainMenuExitParameters
    {
        public readonly SceneEnterParameters TargetSceneEnterParameters;

        public MainMenuExitParameters(SceneEnterParameters targetSceneEnterParameters)
        {
            TargetSceneEnterParameters = targetSceneEnterParameters;
        }
    }
}