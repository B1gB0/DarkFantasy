using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;

namespace _Project.Scripts.Services
{
    public interface IShopService : IService
    {
        public List<PlayerAttributeLevelData> GetAttributesByType(CharacteristicType type);
        public CharacteristicsLocalizationData GetLocalizationDataByType(CharacteristicType type);
        public List<ItemData> GetItemsData();
    }
}