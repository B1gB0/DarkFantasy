using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Level;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services
{
    public interface IProgressionService : IService
    {
        LevelDifficulty CurrentDifficulty { get; }
        
        public void SetDifficulty(LevelDifficulty difficulty);

        public float GetItemMultiplier(LevelDifficulty difficulty);
        public float GetEnemyHealthMultiplier();
        public float GetEnemyDamageMultiplier();
    }
}