using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Enemy;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

namespace _Project.Scripts.Services
{
    public class EnemyService : IEnemyService
    {
        private readonly Dictionary<EnemyType, EnemyData> _enemiesData = new ();

        private IDataBaseService _dataBaseService;

        public bool IsInitiated { get; private set; }

        [Inject]
        public void Construct(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }

        public UniTask Init()
        {
            if (IsInitiated)
                return UniTask.CompletedTask;

            foreach (var enemy in _dataBaseService.Content.Enemies)
            {
                _enemiesData.TryAdd(enemy.Type, enemy);
            }

            IsInitiated = true;

            return UniTask.CompletedTask;
        }

        public EnemyData GetEnemyDataByType(EnemyType type)
        {
            return _enemiesData[type];
        }
    }
}