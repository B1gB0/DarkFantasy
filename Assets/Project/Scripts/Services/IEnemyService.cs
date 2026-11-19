using Project.Scripts.DataBase.InitDataSO;
using Project.Scripts.Enemy;

namespace Project.Scripts.Services
{
    public interface IEnemyService : IService
    {
        public void GetData(EnemyInitData enemyInitData);
        
        public Skeleton CreateSkeleton();
        
        public SkeletonHeavyArmor CreateSkeletonHeavyArmor();
        
        public SkeletonRanger CreateSkeletonRanger();
        
        public Priest CreatePriest();
        
        public Bandit CreateBandit();
        
        public BanditRanger CreateBanditRanger();
        
        public BanditLeader CreateBanditLeader();
        
        public DarkLord CreateDarkLord();
    }
}