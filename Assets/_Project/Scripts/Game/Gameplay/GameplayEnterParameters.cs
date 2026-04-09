using _Project.Scripts.Game.GameRoot;

namespace _Project.Scripts.Game.Gameplay
{
    public class GameplayEnterParameters : SceneEnterParameters
    {
        public GameplayEnterParameters(string sceneName, int currentNumberLevel = 0) : base(sceneName)
        {
            CurrentNumberLevel = currentNumberLevel;
        }

        public int CurrentNumberLevel { get; }
    }
}