using System.Collections.Generic;
using _Project.Scripts.Level;
using UnityEngine;

namespace _Project.Scripts.DataBase.InitDataSO
{
    [CreateAssetMenu(menuName = "Configs/Difficulty Config")]
    public class DifficultyConfig : ScriptableObject
    {
        [SerializeField] private List<DifficultyEntry> _entries = new();

        public DifficultyEntry Get(LevelDifficulty difficulty)
        {
            foreach (var entry in _entries)
                if (entry.Difficulty == difficulty) return entry;

            Debug.LogError($"[Difficulty] Not found: {difficulty}");
            return _entries[0];
        }

        public float GetItemStatMultiplier(LevelDifficulty difficulty) => Get(difficulty).ItemStatMultiplier;
        public float GetEnemyHealthMultiplier(LevelDifficulty difficulty) => Get(difficulty).EnemyHealthMultiplier;
        public float GetEnemyDamageMultiplier(LevelDifficulty difficulty) => Get(difficulty).EnemyDamageMultiplier;
    }
}