using System;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class InventoryItemView : View
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _hoverImage;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private Button _button;

        private IShopService _shopService;

        public ItemData ItemData { get; private set; }
        public Item EquipmentInstance { get; private set; }
        public bool HasEquipmentInstance => EquipmentInstance != null;

        public event Action<InventoryItemView> OnSelectButtonPressed;

        [Inject]
        private void Construct(IShopService shopService)
        {
            _shopService = shopService;
        }

        private void Start()
        {
            _button.onClick.AddListener(OnSelectItem);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnSelectItem);
        }
        
        public void SetEquipment(ItemData template, Item instance)
        {
            ItemData = template;
            EquipmentInstance = instance;

            _hoverImage.gameObject.SetActive(false);

            if (template == null || instance == null)
            {
                Clear();
                return;
            }

            _iconImage.gameObject.SetActive(true);
            _iconImage.sprite = _shopService.GetItemSpriteByType(template.Type);
            
            _count.gameObject.SetActive(false);
        }
        
        public void SetConsumable(ItemData template, int count)
        {
            ItemData = template;
            EquipmentInstance = null;

            _hoverImage.gameObject.SetActive(false);

            if (template == null || count <= 0)
            {
                Clear();
                return;
            }

            _iconImage.gameObject.SetActive(true);
            _iconImage.sprite = _shopService.GetItemSpriteByType(template.Type);

            _count.gameObject.SetActive(true);
            _count.text = count.ToString();
        }
        
        public void Set(ItemData itemData = null, int count = 0)
        {
            if (itemData == null)
            {
                Clear();
                return;
            }

            if (itemData.Kind == ItemKind.Equipment)
                SetEquipment(itemData, null);
            else
                SetConsumable(itemData, count);
        }

        public void ShowHover() => _hoverImage.gameObject.SetActive(true);
        public void HideHover() => _hoverImage.gameObject.SetActive(false);

        private void Clear()
        {
            ItemData = null;
            EquipmentInstance = null;

            _hoverImage.gameObject.SetActive(false);
            _iconImage.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
        }

        private void OnSelectItem()
        {
            if (ItemData == null) return;
            OnSelectButtonPressed?.Invoke(this);
        }
    }
}