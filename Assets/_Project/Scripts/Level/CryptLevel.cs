using _Project.Scripts.Level.Triggers;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class CryptLevel : Level
    {
        [SerializeField] private SpawnTrigger _spawnCyclicWaveTrigger;
        
        private void OnEnable()
        {
            IsInitiatedSpawners += SpawnStartWaves;
            _spawnCyclicWaveTrigger.OnSpawnEnemies += SpawnCyclicWave;
        }
        
        private void FixedUpdate()
        {
            if (_spawnCyclicWaveTrigger.IsEnemySpawned)
            {
                SpawnCyclicWave();
            }
        }

        private void OnDisable()
        {
            IsInitiatedSpawners -= SpawnStartWaves;
            _spawnCyclicWaveTrigger.OnSpawnEnemies += SpawnCyclicWave;
        }

        private void SpawnStartWaves()
        {
            CreateWaveOfDifferentSkeletons(FirstWaveEnemy);
            CreateWaveOfDifferentSkeletons(SecondWaveEnemy);
            CreateWaveOfDifferentSkeletons(ThirdWaveEnemy);
        }

        private void SpawnCyclicWave()
        {
            CreateWaveOfEnemyByTimer(FourthWaveEnemy);
        }
    }
}