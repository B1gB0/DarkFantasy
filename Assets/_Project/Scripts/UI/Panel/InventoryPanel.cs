using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using DG.Tweening;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.Panel
{
    public class InventoryPanel : View.View
    {
        [SerializeField] private Button _backSceneButton;
        [SerializeField] private Button _equipButton;
        
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _name;
        
        [SerializeField] private List<InventoryItemView> _itemViews;
        
        private ITweenAnimationService _tweenAnimationService;
        private IPlayerService _playerService;
        private IInventoryService _inventoryService;
        private IShopService _shopService;
        
        public event Action OnBackToSceneButtonPressed;
        public event Action<ItemType> OnEquipButtonPressed;
        
        [Inject]
        private void Construct(
            ITweenAnimationService tweenAnimationService,
            IShopService shopService,
            IPlayerService playerService,
            IInventoryService inventoryService)
        {
            _tweenAnimationService = tweenAnimationService;
            _shopService = shopService;
            _playerService = playerService;
            _inventoryService = inventoryService;
        }
        
        private void Start()
        {
            _equipButton.interactable = false;
            
            foreach (var itemView in _itemViews)
            {
                itemView.OnSelectButtonPressed += SelectItem;
            }
            
            Deactivate();
        }

        private void OnEnable()
        {
            _backSceneButton.onClick.AddListener(MoveBackToScene);
        }

        private void OnDisable()
        {
            _backSceneButton.onClick.RemoveListener(MoveBackToScene);
        }
        
        private void OnDestroy()
        {
            foreach (var itemView in _itemViews)
            {
                itemView.OnSelectButtonPressed -= SelectItem;
            }
            
            transform.DOKill();
        }
        
        public override void Show()
        {
            _playerService.Player.InputController.LockPlayerMovement();
            
            _tweenAnimationService.AnimateScale(transform);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);
            _playerService.Player.InputController.UnlockPlayerMovement();
        }
        
        private void MoveBackToScene()
        {
            OnBackToSceneButtonPressed?.Invoke();
        }

        private void SelectItem(ItemType type, InventoryItemView  selectedItemView)
        {
            ItemData itemData = GetItemDataByType(type);

            if (itemData == null || !_inventoryService.HasItem(type))
                return;

            foreach (var itemView in _itemViews)
            {
                itemView.HideHover();
            }
            
            SetDescription(itemData);
            selectedItemView.ShowHover();
            _equipButton.interactable = true;

            // Применяем эффект
            // ApplyItemEffect(itemData);

            // Уменьшаем количество
            // _inventoryService.RemoveItem(itemData.Type);

            // Обновить UI
            // RefreshInventoryUI();
        }

        private void SetDescription(ItemData data)
        {
            if (data != null)
            {
                _descriptionText.text = YG2.lang switch
                {
                    LocalizationCode.Ru => data.DescriptionRu,
                    LocalizationCode.En => data.DescriptionEn,
                    LocalizationCode.Tr => data.DescriptionTr,
                    _ => _descriptionText.text
                };
            }
            else
            {
                _descriptionText.text = YG2.lang switch
                {
                    LocalizationCode.Ru => "Предмет не выбран",
                    LocalizationCode.En => "The item is not selected",
                    LocalizationCode.Tr => "Öğe seçilmedi",
                    _ => _descriptionText.text
                };
            }
        }

        private void ApplyItemEffect(ItemData item)
        {
            var characteristics = _playerService.Player.PlayerCharacteristics;

            switch (item.Type)
            {
                case ItemType.SpeedPotion:
                    characteristics.AddSpeedModifier(item.Value, item.Duration, item.IsMultiplier);
                    break;
                case ItemType.HealthPotion:
                    _playerService.Player.Health.AddHealth(item.Value);
                    break;
                case ItemType.Meat:
                    _playerService.Player.Health.AddHealthOverTime(item.Value, item.Duration).Forget();
                    break;
            }
        }

        private ItemData GetItemDataByType(ItemType type)
        {
            return _shopService.GetItemDataByType(type);
        }
    }
}