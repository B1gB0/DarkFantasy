using _Project.Scripts.Level;

namespace _Project.Scripts.Services
{
    public interface IProgressionService : IService
    {
        LevelDifficulty CurrentDifficulty { get; }

        public void SetDifficulty(LevelDifficulty difficulty);
        public float GetItemMultiplier(LevelDifficulty difficulty);
        public float GetEnemyHealthMultiplier();
        public float GetEnemyDamageMultiplier();
        public bool IsDifficultyUnlocked(LevelDifficulty difficulty);
    }
}