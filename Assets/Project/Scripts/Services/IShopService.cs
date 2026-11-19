using System.Collections.Generic;
using Project.Scripts.Characteristics;
using Project.Scripts.DataBase.Data;
using Project.Scripts.Items;
using UnityEngine;

namespace Project.Scripts.Services
{
    public interface IShopService : IService
    {
        public List<PlayerAttributeLevelData> GetAttributesByType(CharacteristicType type);
        
        public CharacteristicsLocalizationData GetLocalizationDataByType(CharacteristicType type);
        
        public string GetLocalizedCharacteristicName(CharacteristicType type);
        
        public List<ItemData> GetItemsData();
        
        public ItemData GetItemDataByType(ItemType type);
        
        public Sprite GetItemSpriteByType(ItemType type);
    }
}