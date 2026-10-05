using System;
using _Project.Scripts.Level;

namespace _Project.Scripts.DataBase.InitDataSO
{
    [Serializable]
    public struct DifficultyEntry
    {
        public LevelDifficulty Difficulty;
        public float EnemyHealthMultiplier;
        public float EnemyDamageMultiplier;
        public float ItemStatMultiplier;
    }
}