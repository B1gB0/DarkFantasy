using _Project.Scripts.DataBase;
using _Project.Scripts.DataBase.InitDataSO;

namespace _Project.Scripts.Services
{
    public interface IDataBaseService : IService
    {
        public PlayerInitData PlayerInitData { get; }
        public EnemyInitData EnemyInitData { get; }
        public SpreadsheetContent Content { get; }
    }
}