using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.DataBase.InitDataSO
{
    [CreateAssetMenu(menuName = "InitData/LevelData")]
    public class LevelInitData : InitData
    {
        [SerializeField] private List<Vector3> _enemyFirstPatrolPositions;
        [SerializeField] private List<Vector3> _enemySecondPatrolPositions;
        [SerializeField] private List<Vector3> _enemyThirdPatrolPositions;
        [SerializeField] private List<Vector3> _enemyFourthPatrolPositions;
        [SerializeField] private List<Vector3> _enemyFifthPatrolPositions;

        [SerializeField] private List<Vector3> _firstWaveSpawnPoints;
        [SerializeField] private List<Vector3> _secondWaveSpawnPoints;
        [SerializeField] private List<Vector3> _thirdWaveSpawnPoints;
        [SerializeField] private List<Vector3> _fourthWaveSpawnPoints;
        [SerializeField] private List<Vector3> _fifthWaveSpawnPoints;

        [SerializeField] private Vector3 _playerSpawnPosition;

        public List<Vector3> EnemyFirstPatrolPositions => _enemyFirstPatrolPositions;
        public List<Vector3> EnemySecondPatrolPositions => _enemySecondPatrolPositions;
        public List<Vector3> EnemyThirdPatrolPositions => _enemyThirdPatrolPositions;
        public List<Vector3> EnemyFourthPatrolPositions => _enemyFourthPatrolPositions;
        public List<Vector3> EnemyFifthPatrolPositions => _enemyFifthPatrolPositions;

        public List<Vector3> FirstWaveSpawnPoints => _firstWaveSpawnPoints;
        public List<Vector3> SecondWaveSpawnPoints => _secondWaveSpawnPoints;
        public List<Vector3> ThirdWaveSpawnPoints => _thirdWaveSpawnPoints;
        public List<Vector3> FourthWaveSpawnPoints => _fourthWaveSpawnPoints;
        public List<Vector3> FifthWaveSpawnPoints => _fifthWaveSpawnPoints;

        public Vector3 PlayerSpawnPosition => _playerSpawnPosition;

        public void SetPlayerSpawnPosition(Vector3 playerSpawnPosition)
        {
            _playerSpawnPosition = playerSpawnPosition;
        }
    }
}