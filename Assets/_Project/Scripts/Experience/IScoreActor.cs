namespace _Project.Scripts.Experience
{
    public interface IScoreActor
    {
        public int Score { get; }
        public bool IsEnemy { get; }
    }
}