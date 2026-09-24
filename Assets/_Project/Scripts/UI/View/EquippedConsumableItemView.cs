using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class EquippedConsumableItemView : View
    {
        private const int MinCount = 0;

        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _count;

        private ItemType _equippedItemType;
        private IPlayerService _playerService;
        private IInventoryService _inventoryService;
        private IShopService _shopService;
        private AudioSoundsService _audioService;

        [Inject]
        private void Construct(
            IPlayerService playerService,
            IInventoryService inventoryService,
            AudioSoundsService audioSoundsService,
            IShopService shopService)
        {
            _playerService = playerService;
            _inventoryService = inventoryService;
            _audioService = audioSoundsService;
            _shopService = shopService;
        }

        private void Start()
        {
            _equippedItemType = YG2.saves.EquippedItemType;

            _iconImage.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
        }

        public void Set(ItemType itemType, int count)
        {
            if (itemType == ItemType.None) return;

            _iconImage.gameObject.SetActive(true);
            _count.gameObject.SetActive(true);

            _equippedItemType = itemType;
            _count.text = count.ToString();

            _iconImage.sprite = _shopService.GetItemSpriteByType(_equippedItemType);
        }

        public void UnSet()
        {
            _equippedItemType = ItemType.None;
            YG2.saves.EquippedItemType = _equippedItemType;
            YG2.SaveProgress();

            _iconImage.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
        }

        public void ApplyItemEffect()
        {
            if (_equippedItemType == ItemType.None) return;

            int currentCount = _inventoryService.GetItemCount(_equippedItemType);
            if (currentCount <= MinCount) return;

            var characteristics = _playerService.Player.PlayerCharacteristics;
            bool effectApplied = false;

            ItemData data = _shopService.GetItemDataByType(_equippedItemType);

            switch (_equippedItemType)
            {
                case ItemType.SpeedPotion:
                    effectApplied = characteristics.AddSpeedModifier(
                        data.Value,
                        data.Duration,
                        data.IsMultiplier);
                    break;
                case ItemType.HealthPotion:
                    _playerService.Player.Health.AddHealth(data.Value);
                    effectApplied = true;
                    break;
                case ItemType.Meat:
                    effectApplied = _playerService.Player.Health.TryStartHealingOverTime(
                        data.Value,
                        data.Duration);
                    break;
            }

            if (!effectApplied)
                return;

            _audioService.PlaySound(SoundsType.PotionSound).Forget();
            _inventoryService.RemoveItem(_equippedItemType);

            int newCount = _inventoryService.GetItemCount(_equippedItemType);
            if (newCount > MinCount)
            {
                Set(_equippedItemType, newCount);
            }
            else
            {
                UnSet();
            }
        }
    }
}