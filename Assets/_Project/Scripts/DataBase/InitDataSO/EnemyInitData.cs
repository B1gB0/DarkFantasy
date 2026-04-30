using _Project.Scripts.Enemy;
using _Project.Scripts.Projectile;
using UnityEngine;

namespace _Project.Scripts.DataBase.InitDataSO
{
    [CreateAssetMenu(menuName = "InitData/EnemyInitData")]
    public class EnemyInitData : InitData
    {
        [field: SerializeField] public Skeleton SkeletonPrefab { get; private set; }
        [field: SerializeField] public SkeletonFlesh SkeletonFleshPrefab { get; private set; }
        [field: SerializeField] public SkeletonRanger SkeletonRangerPrefab { get; private set; }
        [field: SerializeField] public SkeletonLightArmor SkeletonLightArmorPrefab { get; private set; }
        [field: SerializeField] public SkeletonHeavyArmor SkeletonHeavyArmorPrefab { get; private set; }
        [field: SerializeField] public Priest PriestPrefab { get; private set; }
        [field: SerializeField] public Arrow ArrowProjectilePrefab { get; private set; }
        [field: SerializeField] public Fireball FireballProjectilePrefab { get; private set; }
    }
}