#if UNITY_EDITOR
using UnityEngine;

namespace _Project.Scripts.DataBase.InitDataSO
{
    public class LevelDataForConfig : MonoBehaviour
    {
        [SerializeField] private LevelInitData levelInitData;

        [ContextMenu("Save Data")]
        public void SaveDataToConfigLevel()
        {
            GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");
            
            GameObject[] enemyFirstPatrolPoints = GameObject.FindGameObjectsWithTag("EnemyFirstPatrolPoints");
            GameObject[] enemySecondPatrolPoints = GameObject.FindGameObjectsWithTag("EnemySecondPatrolPoints");
            GameObject[] enemyThirdPatrolPoints = GameObject.FindGameObjectsWithTag("EnemyThirdPatrolPoints");
            GameObject[] enemyFourthPatrolPoints = GameObject.FindGameObjectsWithTag("EnemyFourthPatrolPoints");
            
            GameObject[] firstWavePoints = GameObject.FindGameObjectsWithTag("FirstWaveEnemySpawnPoints");
            GameObject[] secondWavePoints = GameObject.FindGameObjectsWithTag("SecondWaveEnemySpawnPoints");
            GameObject[] thirdWavePoints = GameObject.FindGameObjectsWithTag("ThirdWaveEnemySpawnPoints");
            GameObject[] fourthWavePoints = GameObject.FindGameObjectsWithTag("FourthWaveEnemySpawnPoints");

            levelInitData.PlayerSpawnPosition = playerSpawnPoint.transform.position;
            
            levelInitData.EnemyFirstPatrolPositions.Clear();
            levelInitData.EnemySecondPatrolPositions.Clear();
            levelInitData.EnemyThirdPatrolPositions.Clear();
            levelInitData.EnemyFourthPatrolPositions.Clear();
            
            levelInitData.FirstWaveSpawnPoints.Clear();
            levelInitData.SecondWaveSpawnPoints.Clear();
            levelInitData.ThirdWaveSpawnPoints.Clear();
            levelInitData.FourthWaveSpawnPoints.Clear();

            foreach (var point in enemyFirstPatrolPoints)
            {
                levelInitData.EnemyFirstPatrolPositions.Add(point.transform.position);
            }
            
            foreach (var point in enemySecondPatrolPoints)
            {
                levelInitData.EnemySecondPatrolPositions.Add(point.transform.position);
            }
            
            foreach (var point in enemyThirdPatrolPoints)
            {
                levelInitData.EnemyThirdPatrolPositions.Add(point.transform.position);
            }
            
            foreach (var point in enemyFourthPatrolPoints)
            {
                levelInitData.EnemyFourthPatrolPositions.Add(point.transform.position);
            }

            foreach (var point in firstWavePoints)
            {
                levelInitData.FirstWaveSpawnPoints.Add(point.transform.position);
            }

            foreach (var point in secondWavePoints)
            {
                levelInitData.SecondWaveSpawnPoints.Add(point.transform.position);
            }
            
            foreach (var point in thirdWavePoints)
            {
                levelInitData.ThirdWaveSpawnPoints.Add(point.transform.position);
            }
            
            foreach (var point in fourthWavePoints)
            {
                levelInitData.FourthWaveSpawnPoints.Add(point.transform.position);
            }

            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(levelInitData);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif