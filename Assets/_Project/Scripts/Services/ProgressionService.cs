using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Level;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using YG;

namespace _Project.Scripts.Services
{
    public class ProgressionService : IProgressionService
    {
        private const string DifficultyConfigPath = "DifficultyConfig";
        
        private DifficultyConfig _config;
        private IResourceService _resourceService;

        public bool IsInitiated { get; private set; }
        public LevelDifficulty CurrentDifficulty { get; private set; } = LevelDifficulty.Low;

        [Inject]
        public void Construct(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }
        
        public async UniTask Init()
        {
            if (IsInitiated) return;

            CurrentDifficulty = YG2.saves.CurrentDifficulty;
            
            _config = await _resourceService.Load<DifficultyConfig>(DifficultyConfigPath);
            
            IsInitiated = true;
        }

        public void SetDifficulty(LevelDifficulty difficulty)
        {
            CurrentDifficulty = difficulty;
            YG2.saves.CurrentDifficulty = difficulty;
            YG2.SaveProgress();
        }

        public float GetItemMultiplier(LevelDifficulty difficulty)
        {
            return _config.Get(difficulty).ItemStatMultiplier;
        }

        public float GetEnemyHealthMultiplier()
        {
            return _config.Get(CurrentDifficulty).EnemyHealthMultiplier;
        }

        public float GetEnemyDamageMultiplier()
        {
            return _config.Get(CurrentDifficulty).EnemyDamageMultiplier;
        }
        
        public bool IsDifficultyUnlocked(LevelDifficulty difficulty)
        {
            return difficulty switch
            {
                LevelDifficulty.Low   => true,
                LevelDifficulty.Medium => YG2.saves.IsMediumDifficultyUnlock,
                LevelDifficulty.High   => YG2.saves.IsHardDifficultyUnlock,
                _ => false,
            };
        }
    }
}