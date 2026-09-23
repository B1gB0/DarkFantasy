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
        private IInventoryService _inventoryService;

        [Inject]
        private void Construct(IShopService shopService, IInventoryService inventoryService)
        {
            _shopService = shopService;
            _inventoryService = inventoryService;
        }

        private void OnEnable()
        {
            _inventoryService.OnEquippedItem += Refresh;
            _inventoryService.OnUnEquippedItem += Refresh;

            Refresh();
        }

        private void OnDisable()
        {
            _inventoryService.OnEquippedItem -= Refresh;
            _inventoryService.OnUnEquippedItem -= Refresh;
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
                ItemRarity.Common => Color.white,
                ItemRarity.Uncommon => Color.green,
                ItemRarity.Rare => Color.blue,
                _ => Color.white,
            };

            _icon.sprite = _shopService.GetItemSpriteByType(data.Type);
        }

        private void Refresh()
        {
            ItemType type = GetEquippedTypeForMySlot();
            ItemData data = type == ItemType.None
                ? null
                : _shopService.GetItemDataByType(type);

            Set(data);
        }

        private ItemType GetEquippedTypeForMySlot()
        {
            return _equipmentType switch
            {
                EquipmentType.Weapon => YG2.saves.EquipedWeaponType,
                EquipmentType.Armor  => YG2.saves.EquipedArmorType,
                EquipmentType.Ring   => YG2.saves.EquipedRingType,
                _ => ItemType.None,
            };
        }
    }
}