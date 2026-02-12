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
            // GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");
            
            GameObject[] enemyPatrolPoints = GameObject.FindGameObjectsWithTag("EnemyPatrolPoints");
            
            GameObject[] skeletonPoints = GameObject.FindGameObjectsWithTag("SkeletonPoints");
            GameObject[] skeletonFleshPoints = GameObject.FindGameObjectsWithTag("SkeletonFleshPoints");
            GameObject[] skeletonHeavyArmorPoints = GameObject.FindGameObjectsWithTag("SkeletonHeavyArmorPoints");
            GameObject[] skeletonLightArmorPoints = GameObject.FindGameObjectsWithTag("SkeletonLightArmorPoints");
            GameObject[] skeletonRangerPoints = GameObject.FindGameObjectsWithTag("SkeletonRangerPoints");
            
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

            // levelInitData.PlayerSpawnPosition = playerSpawnPoint.transform.position;
            
            levelInitData.EnemyPatrolPositions.Clear();
            
            levelInitData.SkeletonEnemySpawnPositions.Clear();
            levelInitData.SkeletonFleshEnemySpawnPositions.Clear();
            levelInitData.SkeletonHeavyArmorEnemySpawnPositions.Clear();
            levelInitData.SkeletonLightArmorEnemySpawnPositions.Clear();
            levelInitData.SkeletonRangerEnemySpawnPositions.Clear();
            
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
            
            foreach (var point in enemyPatrolPoints)
            {
                levelInitData.EnemyPatrolPositions.Add(point.transform.position);
            }

            foreach (var point in skeletonPoints)
            {
                levelInitData.SkeletonEnemySpawnPositions.Add(point.transform.position);
            }

            foreach (var point in skeletonFleshPoints)
            {
                levelInitData.SkeletonFleshEnemySpawnPositions.Add(point.transform.position);
            }
            
            foreach (var point in skeletonHeavyArmorPoints)
            {
                levelInitData.SkeletonHeavyArmorEnemySpawnPositions.Add(point.transform.position);
            }
            
            foreach (var point in skeletonLightArmorPoints)
            {
                levelInitData.SkeletonLightArmorEnemySpawnPositions.Add(point.transform.position);
            }
            
            foreach (var point in skeletonRangerPoints)
            {
                levelInitData.SkeletonRangerEnemySpawnPositions.Add(point.transform.position);
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