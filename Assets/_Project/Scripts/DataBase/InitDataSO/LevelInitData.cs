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
        public List<Vector3> EnemyFourthPatrolPositions;

        public List<Vector3> FirstWaveSpawnPoints;
        public List<Vector3> SecondWaveSpawnPoints;
        public List<Vector3> ThirdWaveSpawnPoints;
        public List<Vector3> FourthWaveSpawnPoints;

        public Vector3 PlayerSpawnPosition;
    }
}