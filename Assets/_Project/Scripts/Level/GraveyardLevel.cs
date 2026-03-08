using _Project.Scripts.Level.Triggers;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class GraveyardLevel : Level
    {
        [SerializeField] private SpawnTrigger _spawnLastWaveTrigger;
        
        private void OnEnable()
        {
            IsInitiatedSpawners += SpawnStartWaves;
            _spawnLastWaveTrigger.OnSpawnEnemies += SpawnLastWave;
        }

        private void OnDisable()
        {
            IsInitiatedSpawners -= SpawnStartWaves;
            _spawnLastWaveTrigger.OnSpawnEnemies -= SpawnLastWave;
        }

        private void SpawnStartWaves()
        {
            CreateWaveOfEnemies(FirstWaveEnemy);
            CreateWaveOfEnemies(SecondWaveEnemy);
            CreateWaveOfEnemies(ThirdWaveEnemy);
        }

        private void SpawnLastWave()
        {
            CreateWaveOfEnemies(FourthWaveEnemy);
        }
    }
}