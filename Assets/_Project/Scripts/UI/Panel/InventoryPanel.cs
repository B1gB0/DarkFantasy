using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Panel
{
    public class InventoryPanel : View.View
    {
        [SerializeField] private Button _backSceneButton;
        
        private ITweenAnimationService _tweenAnimationService;
        private IPlayerService _playerService;
        private IInventoryService _inventoryService;
        private IShopService _shopService;
        
        private List<ShopItemView> _itemViews;

        public event Action OnBackToSceneButtonPressed;
        
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
            transform.DOKill();
        }
        
        public override void Show()
        {
            // foreach (var itemView in _itemViews)
            // {
            //     itemView.OnButtonClicked += ApplyPurchase;
            // }
            
            _playerService.Player.InputController.LockPlayerMovement();
            
            _tweenAnimationService.AnimateScale(transform);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);
            _playerService.Player.InputController.UnlockPlayerMovement();
        }
        
        public void OnChangeLanguage()
        {
            foreach (ShopItemView itemView in _itemViews)
            {
                itemView.SetLocalization();
            }
        }
        
        private void MoveBackToScene()
        {
            OnBackToSceneButtonPressed?.Invoke();
        }

        public void UseItem(ItemType type)
        {
            ItemData itemData = GetItemDataByType(type);

            if (itemData == null || !_inventoryService.HasItem(type))
                return;

            // Применяем эффект
            ApplyItemEffect(itemData);

            // Уменьшаем количество
            _inventoryService.RemoveItem(itemData.Type);

            // Обновить UI
            // RefreshInventoryUI();
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