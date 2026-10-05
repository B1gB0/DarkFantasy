using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class SlotView : View
    {
        [SerializeField] private EquipmentType _equipmentType;
        [SerializeField] private Image _rarityBackground;
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _defaultSprite;

        private IShopService _shopService;
        private IInventoryService _inventoryService;

        [Inject]
        private void Construct(IShopService shopService, IInventoryService inventoryService)
        {
            _shopService = shopService;
            _inventoryService = inventoryService;
        }

        private void OnEnable()
        {
            if (_inventoryService == null) return;

            _inventoryService.OnEquippedItem += Refresh;
            _inventoryService.OnUnEquippedItem += Refresh;

            if (_shopService.IsInitiated)
                Refresh();
        }

        private void OnDisable()
        {
            if (_inventoryService == null) return;

            _inventoryService.OnEquippedItem -= Refresh;
            _inventoryService.OnUnEquippedItem -= Refresh;
        }

        private void Refresh()
        {
            // Получаем НАДЕТЫЙ ЭКЗЕМПЛЯР для своего слота
            Item instance = GetEquippedInstanceForMySlot();

            if (instance == null)
            {
                Set(null);
                return;
            }

            var data = _shopService.GetItemDataByType(instance.Type);
            if (data == null)
            {
                Set(null);
                return;
            }

            Set(data);
        }

        private Item GetEquippedInstanceForMySlot()
        {
            return _equipmentType switch
            {
                EquipmentType.Weapon => _inventoryService.EquippedWeapon,
                EquipmentType.Armor  => _inventoryService.EquippedArmor,
                EquipmentType.Ring   => _inventoryService.EquippedRing,
                _ => null,
            };
        }

        private void Set(ItemData data)
        {
            if (data == null)
            {
                _rarityBackground.color = Color.white;
                _icon.sprite = _defaultSprite;
                return;
            }

            _rarityBackground.color = data.Rarity switch
            {
                ItemRarity.Common   => Color.white,
                ItemRarity.Uncommon => Color.green,
                ItemRarity.Rare     => Color.blue,
                _ => Color.white,
            };

            _icon.sprite = _shopService.GetItemSpriteByType(data.Type);
        }
    }
}