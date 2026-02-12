using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.DataBase.InitDataSO
{
    [CreateAssetMenu(menuName = "InitData/LevelData")]
    public class LevelInitData : InitData
    {
        public List<Vector3> EnemyPatrolPositions;

        public List<Vector3> SkeletonEnemySpawnPositions;
        public List<Vector3> SkeletonFleshEnemySpawnPositions;
        public List<Vector3> SkeletonHeavyArmorEnemySpawnPositions;
        public List<Vector3> SkeletonLightArmorEnemySpawnPositions;
        public List<Vector3> SkeletonRangerEnemySpawnPositions;

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