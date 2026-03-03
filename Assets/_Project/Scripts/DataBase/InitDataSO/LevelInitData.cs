using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.DataBase.InitDataSO
{
    [CreateAssetMenu(menuName = "InitData/LevelData")]
    public class LevelInitData : InitData
    {
        public List<Vector3> EnemyFirstPatrolPositions;
        public List<Vector3> EnemySecondPatrolPositions;
        public List<Vector3> EnemyThirdPatrolPositions;

        public List<Vector3> FirstWaveSpawnPoints;
        public List<Vector3> ThirdWaveSpawnPoints;
        public List<Vector3> SecondWaveSpawnPoints;

        public Vector3 PlayerSpawnPosition;

        // public List<Vector3> FirstWaveSmallEnemyAlienSpawnPositions;
        // public List<Vector3> FirstWaveBigEnemyAlienSpawnPositions;
        // public List<Vector3> FirstWaveGunnerEnemyAlienSpawnPositions;
        //
        // public List<Vector3> SecondWaveSmallEnemyAlienSpawnPositions;
        // public List<Vector3> SecondWaveBigEnemyAlienSpawnPositions;
        // public List<Vector3> SecondWaveGunnerEnemyAlienSpawnPositions;
        //
        // public List<Vector3> EnemyTurretsSpawnPoints;
        //
        // public List<Vector3> AlienCocoonSpawnPoints;
        // public List<Vector3> StoneSpawnPositions;
        // public List<Vector3> GoldCoreSpawnPositions;
        // public List<Vector3> HealingCoreSpawnPositions;
        // public List<Vector3> IceCrystalsSpawnPositions;
    }
}