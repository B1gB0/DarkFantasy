using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class ShopService : IShopService
    {
        public const string IconsConfigPath = "IconData";

        private readonly Dictionary<string, PlayerAttributeLevelData> _attributesData = new();

        private readonly Dictionary<CharacteristicType, CharacteristicsLocalizationData>
            _characteristicsLocalizationData = new();

        private readonly Dictionary<ItemType, ItemData> _itemsData = new();

        private IDataBaseService _dataBaseService;
        private IResourceService _resourceService;
        private IconsConfig _iconsConfig;

        [Inject]
        public void Construct(IDataBaseService dataBaseService, IResourceService resourceService)
        {
            _dataBaseService = dataBaseService;
            _resourceService = resourceService;
        }

        public bool IsInitiated { get; private set; }

        public async UniTask Init()
        {
            if (IsInitiated)
                return;

            foreach (var attributeData in _dataBaseService.Content.PlayerAttributeLevelData)
            {
                _attributesData.TryAdd(attributeData.Id, attributeData);
            }

            foreach (var localizationData in _dataBaseService.Content.CharacteristicsLocalizationData)
            {
                _characteristicsLocalizationData.TryAdd(localizationData.Type, localizationData);
            }

            foreach (var itemData in _dataBaseService.Content.ItemsData)
            {
                _itemsData.TryAdd(itemData.Type, itemData);
            }

            _iconsConfig = await _resourceService.Load<IconsConfig>(IconsConfigPath);

            IsInitiated = true;
        }

        public List<PlayerAttributeLevelData> GetAttributesByType(CharacteristicType type)
        {
            List<PlayerAttributeLevelData> attributesData = new List<PlayerAttributeLevelData>();

            foreach (var attributeData in _attributesData)
            {
                if (attributeData.Value.Type == type)
                    attributesData.Add(attributeData.Value);
            }

            return attributesData;
        }

        public CharacteristicsLocalizationData GetLocalizationDataByType(CharacteristicType type)
        {
            return _characteristicsLocalizationData[type];
        }

        public ItemData GetItemDataByType(ItemType type)
        {
            return _itemsData[type];
        }
        
        public Sprite GetItemSpriteByType(ItemType type)
        {
            return _iconsConfig.Get(type);
        }

        public List<ItemData> GetItemsData()
        {
            return _itemsData.Select(itemData => itemData.Value).ToList();
        }
    }
}