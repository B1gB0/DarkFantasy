namespace _Project.Scripts.Experience
{
    public interface IExperiencePoints
    {
        public int AccumulatedKills { get; }
        public int AccumulatedScore { get; }
        public void OnKill(IAcceptable experience);
        public void ResetAccumulatedValues();
    }
}