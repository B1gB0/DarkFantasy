using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public interface IShopService : IService
    {
        public List<PlayerAttributeLevelData> GetAttributesByType(CharacteristicType type);
        public CharacteristicsLocalizationData GetLocalizationDataByType(CharacteristicType type);
        public List<ItemData> GetItemsData();
        public ItemData GetItemDataByType(ItemType type);
        public Sprite GetItemSpriteByType(ItemType type);
    }
}