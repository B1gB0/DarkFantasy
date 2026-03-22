using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

namespace _Project.Scripts.Services
{
    public class ShopService : IShopService
    {
        private readonly Dictionary<string, PlayerAttributeLevelData> _attributesData = new();

        private IDataBaseService _dataBaseService;

        [Inject]
        public void Construct(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }

        public bool IsInitiated { get; private set; }

        public UniTask Init()
        {
            if (IsInitiated)
                return UniTask.CompletedTask;

            foreach (var data in _dataBaseService.Content.PlayerAttributeLevelData)
            {
                _attributesData.TryAdd(data.Id, data);
            }

            IsInitiated = true;

            return UniTask.CompletedTask;
        }

        //public void GetAttribute()
    }
}