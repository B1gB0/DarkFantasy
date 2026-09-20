using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class SlotView : View
    {
        [SerializeField] private EquipmentType _equipmentType;
        [SerializeField] private Image _rarityBackground;
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _defaultSprite;

        private IShopService _shopService;

        [Inject]
        private void Construct(IShopService shopService)
        {
            _shopService = shopService;
        }

        private void Start()
        {
            LoadSlots();
        }

        public void Set(ItemData data)
        {
            if (data == null)
            {
                _rarityBackground.color = Color.white;
                _icon.sprite = _defaultSprite;
                return;
            }
            
            _rarityBackground.color = data.Rarity switch
            {
                ItemRarity.Common => Color.white,
                ItemRarity.Uncommon => Color.green,
                ItemRarity.Rare => Color.blue,
                _ => _rarityBackground.color
            };
            
            _icon.sprite = _shopService.GetItemSpriteByType(data.Type);
        }
        
        private void LoadSlots()
        {
            switch (_equipmentType)
            {
                case EquipmentType.Armor:
                {
                    if (YG2.saves.EquipedArmorType != ItemType.None)
                    {
                        Set(_shopService.GetItemDataByType(YG2.saves.EquipedArmorType));
                    }
                    break;
                }
                case EquipmentType.Weapon:
                {
                    if (YG2.saves.EquipedWeaponType != ItemType.None)
                    {
                        Set(_shopService.GetItemDataByType(YG2.saves.EquipedWeaponType));
                    }
                    break;
                }
                case EquipmentType.Ring:
                {
                    if (YG2.saves.EquipedRingType != ItemType.None)
                    {
                        Set(_shopService.GetItemDataByType(YG2.saves.EquipedRingType));
                    }
                    break;
                }
            }
        }
    }
}