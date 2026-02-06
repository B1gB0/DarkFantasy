using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Enemy;

namespace _Project.Scripts.Services
{
    public interface IEnemyService : IService
    {
        public EnemyData GetEnemyDataByType(EnemyType type);
    }
}