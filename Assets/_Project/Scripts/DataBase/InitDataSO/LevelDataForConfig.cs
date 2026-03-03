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
            
            GameObject[] firstWavePoints = GameObject.FindGameObjectsWithTag("FirstWaveEnemySpawnPoints");
            GameObject[] secondWavePoints = GameObject.FindGameObjectsWithTag("SecondWaveEnemySpawnPoints");
            GameObject[] thirdWavePoints = GameObject.FindGameObjectsWithTag("ThirdWaveEnemySpawnPoints");

            // GameObject[] firstWaveSmallEnemy = GameObject.FindGameObjectsWithTag("FirstWaveSmallEnemyAlienSpawnPoint");
            // GameObject[] firstWaveBigEnemy = GameObject.FindGameObjectsWithTag("FirstWaveBigEnemyAlienSpawnPoint");
            // GameObject[] firstWaveGunnerEnemy =
            //     GameObject.FindGameObjectsWithTag("FirstWaveGunnerEnemyAlienSpawnPoint");
            //
            // GameObject[] secondWaveSmallEnemy =
            //     GameObject.FindGameObjectsWithTag("SecondWaveSmallEnemyAlienSpawnPoint");
            // GameObject[] secondWaveBigEnemy = GameObject.FindGameObjectsWithTag("SecondWaveBigEnemyAlienSpawnPoint");
            // GameObject[] secondWaveGunnerEnemy =
            //     GameObject.FindGameObjectsWithTag("SecondWaveGunnerEnemyAlienSpawnPoint");
            //
            // GameObject[] enemyTurretSpawnPoints =
            //     GameObject.FindGameObjectsWithTag("EnemyTurretSpawnPoints");
            //
            // GameObject[] alienCocoonSpawnPoints = GameObject.FindGameObjectsWithTag("AlienCocoonSpawnPoints");
            // GameObject[] stoneSpawnPoints = GameObject.FindGameObjectsWithTag("StoneSpawnPoint");
            // GameObject[] healingCoreSpawnPoints = GameObject.FindGameObjectsWithTag("HealingCoreSpawnPoint");
            // GameObject[] iceCrystalSpawnPoints = GameObject.FindGameObjectsWithTag("IceCrystalSpawnPoint");
            // GameObject[] goldCoreSpawnPoints = GameObject.FindGameObjectsWithTag("GoldCoreSpawnPoint");

            levelInitData.PlayerSpawnPosition = playerSpawnPoint.transform.position;
            
            levelInitData.EnemyFirstPatrolPositions.Clear();
            levelInitData.EnemySecondPatrolPositions.Clear();
            levelInitData.EnemyThirdPatrolPositions.Clear();
            
            levelInitData.FirstWaveSpawnPoints.Clear();
            levelInitData.SecondWaveSpawnPoints.Clear();
            levelInitData.ThirdWaveSpawnPoints.Clear();

            // levelInitData.FirstWaveSmallEnemyAlienSpawnPositions.Clear();
            // levelInitData.FirstWaveBigEnemyAlienSpawnPositions.Clear();
            // levelInitData.FirstWaveGunnerEnemyAlienSpawnPositions.Clear();
            // levelInitData.SecondWaveSmallEnemyAlienSpawnPositions.Clear();
            // levelInitData.SecondWaveBigEnemyAlienSpawnPositions.Clear();
            // levelInitData.SecondWaveGunnerEnemyAlienSpawnPositions.Clear();
            // levelInitData.StoneSpawnPositions.Clear();
            // levelInitData.HealingCoreSpawnPositions.Clear();
            // levelInitData.GoldCoreSpawnPositions.Clear();
            // levelInitData.AlienCocoonSpawnPoints.Clear();
            // levelInitData.EnemyTurretsSpawnPoints.Clear();
            // levelInitData.IceCrystalsSpawnPositions.Clear();
            //
            
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

            // foreach (var point in firstWaveSmallEnemy)
            // {
            //     levelInitData.FirstWaveSmallEnemyAlienSpawnPositions.Add(point.transform.position);
            // }
            //
            // foreach (var point in firstWaveBigEnemy)
            // {
            //     levelInitData.FirstWaveBigEnemyAlienSpawnPositions.Add(point.transform.position);
            // }
            //
            // foreach (var point in firstWaveGunnerEnemy)
            // {
            //     levelInitData.FirstWaveGunnerEnemyAlienSpawnPositions.Add(point.transform.position);
            // }
            //
            // foreach (var point in secondWaveSmallEnemy)
            // {
            //     levelInitData.SecondWaveSmallEnemyAlienSpawnPositions.Add(point.transform.position);
            // }
            //
            // foreach (var point in secondWaveBigEnemy)
            // {
            //     levelInitData.SecondWaveBigEnemyAlienSpawnPositions.Add(point.transform.position);
            // }
            //
            // foreach (var point in secondWaveGunnerEnemy)
            // {
            //     levelInitData.SecondWaveGunnerEnemyAlienSpawnPositions.Add(point.transform.position);
            // }
            //
            // foreach (var point in enemyTurretSpawnPoints)
            // {
            //     levelInitData.EnemyTurretsSpawnPoints.Add(point.transform.position);
            // }
            //
            // foreach (var point in alienCocoonSpawnPoints)
            // {
            //     levelInitData.AlienCocoonSpawnPoints.Add(point.transform.position);
            // }
            //
            // foreach (var stone in stoneSpawnPoints)
            // {
            //     levelInitData.StoneSpawnPositions.Add(stone.transform.position);
            // }
            //
            // foreach (var healingCore in healingCoreSpawnPoints)
            // {
            //     levelInitData.HealingCoreSpawnPositions.Add(healingCore.transform.position);
            // }
            //
            // foreach (var goldCore in goldCoreSpawnPoints)
            // {
            //     levelInitData.GoldCoreSpawnPositions.Add(goldCore.transform.position);
            // }
            //
            // foreach (var iceCrystal in iceCrystalSpawnPoints)
            // {
            //     levelInitData.IceCrystalsSpawnPositions.Add(iceCrystal.transform.position);
            // }

            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(levelInitData);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif