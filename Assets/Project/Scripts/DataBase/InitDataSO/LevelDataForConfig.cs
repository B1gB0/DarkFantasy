#if UNITY_EDITOR
using UnityEngine;

namespace Project.Scripts.DataBase.InitDataSO
{
    public class LevelDataForConfig : MonoBehaviour
    {
        [SerializeField] private LevelInitData _levelInitData;

        [ContextMenu("Save Data")]
        public void SaveDataToConfigLevel()
        {
            GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");

            GameObject[] enemyFirstPatrolPoints = GameObject.FindGameObjectsWithTag("EnemyFirstPatrolPoints");
            GameObject[] enemySecondPatrolPoints = GameObject.FindGameObjectsWithTag("EnemySecondPatrolPoints");
            GameObject[] enemyThirdPatrolPoints = GameObject.FindGameObjectsWithTag("EnemyThirdPatrolPoints");
            GameObject[] enemyFourthPatrolPoints = GameObject.FindGameObjectsWithTag("EnemyFourthPatrolPoints");
            GameObject[] enemyFifthPatrolPoints = GameObject.FindGameObjectsWithTag("EnemyFifthPatrolPoints");

            GameObject[] firstWavePoints = GameObject.FindGameObjectsWithTag("FirstWaveEnemySpawnPoints");
            GameObject[] secondWavePoints = GameObject.FindGameObjectsWithTag("SecondWaveEnemySpawnPoints");
            GameObject[] thirdWavePoints = GameObject.FindGameObjectsWithTag("ThirdWaveEnemySpawnPoints");
            GameObject[] fourthWavePoints = GameObject.FindGameObjectsWithTag("FourthWaveEnemySpawnPoints");
            GameObject[] fifthWavePoints = GameObject.FindGameObjectsWithTag("FifthWaveEnemySpawnPoints");

            _levelInitData.SetPlayerSpawnPosition(playerSpawnPoint.transform.position);

            _levelInitData.EnemyFirstPatrolPositions.Clear();
            _levelInitData.EnemySecondPatrolPositions.Clear();
            _levelInitData.EnemyThirdPatrolPositions.Clear();
            _levelInitData.EnemyFourthPatrolPositions.Clear();
            _levelInitData.EnemyFifthPatrolPositions.Clear();

            _levelInitData.FirstWaveSpawnPoints.Clear();
            _levelInitData.SecondWaveSpawnPoints.Clear();
            _levelInitData.ThirdWaveSpawnPoints.Clear();
            _levelInitData.FourthWaveSpawnPoints.Clear();
            _levelInitData.FifthWaveSpawnPoints.Clear();

            foreach (var point in enemyFirstPatrolPoints)
            {
                _levelInitData.EnemyFirstPatrolPositions.Add(point.transform.position);
            }

            foreach (var point in enemySecondPatrolPoints)
            {
                _levelInitData.EnemySecondPatrolPositions.Add(point.transform.position);
            }

            foreach (var point in enemyThirdPatrolPoints)
            {
                _levelInitData.EnemyThirdPatrolPositions.Add(point.transform.position);
            }

            foreach (var point in enemyFourthPatrolPoints)
            {
                _levelInitData.EnemyFourthPatrolPositions.Add(point.transform.position);
            }

            foreach (var point in enemyFifthPatrolPoints)
            {
                _levelInitData.EnemyFifthPatrolPositions.Add(point.transform.position);
            }

            foreach (var point in firstWavePoints)
            {
                _levelInitData.FirstWaveSpawnPoints.Add(point.transform.position);
            }

            foreach (var point in secondWavePoints)
            {
                _levelInitData.SecondWaveSpawnPoints.Add(point.transform.position);
            }

            foreach (var point in thirdWavePoints)
            {
                _levelInitData.ThirdWaveSpawnPoints.Add(point.transform.position);
            }

            foreach (var point in fourthWavePoints)
            {
                _levelInitData.FourthWaveSpawnPoints.Add(point.transform.position);
            }

            foreach (var point in fifthWavePoints)
            {
                _levelInitData.FifthWaveSpawnPoints.Add(point.transform.position);
            }

            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(_levelInitData);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif