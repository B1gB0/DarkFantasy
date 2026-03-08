using System;

namespace _Project.Scripts.Level
{
    public class GenaLevel : Level
    {
        private void OnEnable()
        {
            IsInitiatedSpawners += SpawnStartWaves;
        }

        private void OnDisable()
        {
            IsInitiatedSpawners -= SpawnStartWaves;
        }

        private void SpawnStartWaves()
        {
            CreateWaveOfEnemies(FirstWaveEnemy);
        }
    }
}