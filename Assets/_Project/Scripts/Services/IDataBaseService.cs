using _Project.Scripts.DataBase;

namespace _Project.Scripts.Services
{
    public interface IDataBaseService : IService
    {
        SpreadsheetContent Content { get; }
    }
}