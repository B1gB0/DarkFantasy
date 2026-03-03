using _Project.Scripts.DataBase.Data;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Enemy;

namespace _Project.Scripts.Services
{
    public interface IEnemyService : IService
    {
        public void GetData(SkeletonInitData skeletonInitData);
        public EnemyData GetEnemyDataByType(EnemyType type);
        public Skeleton CreateSkeleton();
        public SkeletonHeavyArmor CreateSkeletonHeavyArmor();
        public SkeletonRanger CreateSkeletonRanger();
    }
}