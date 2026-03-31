using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

namespace _Project.Scripts.Services
{
    public class ShopService : IShopService
    {
        private readonly Dictionary<string, PlayerAttributeLevelData> _attributesData = new();
        private readonly Dictionary<CharacteristicType, CharacteristicsLocalizationData>
            _characteristicsLocalizationData = new();

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

            foreach (var attributeData in _dataBaseService.Content.PlayerAttributeLevelData)
            {
                _attributesData.TryAdd(attributeData.Id, attributeData);
            }

            foreach (var localizationData in _dataBaseService.Content.CharacteristicsLocalizationData)
            {
                _characteristicsLocalizationData.TryAdd(localizationData.Type, localizationData);
            }

            IsInitiated = true;

            return UniTask.CompletedTask;
        }

        public List<PlayerAttributeLevelData> GetAttributesByType(CharacteristicType type)
        {
            List<PlayerAttributeLevelData> attributesData = new List<PlayerAttributeLevelData>();

            foreach (var attributeData in _attributesData)
            {
                if(attributeData.Value.Type == type)
                    attributesData.Add(attributeData.Value);
            }

            return attributesData;
        }

        public CharacteristicsLocalizationData GetLocalizationDataByType(CharacteristicType type)
        {
            return _characteristicsLocalizationData[type];
        }
    }
}