using System;
using Project.Scripts.Level;

namespace Project.Scripts.DataBase.InitDataSO
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