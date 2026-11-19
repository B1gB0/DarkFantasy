using Project.Scripts.DataBase;
using Project.Scripts.DataBase.InitDataSO;

namespace Project.Scripts.Services
{
    public interface IDataBaseService : IService
    {
        public PlayerInitData PlayerInitData { get; }
        public EnemyInitData EnemyInitData { get; }
        public SpreadsheetContent Content { get; }
    }
}