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
using YG;

namespace _Project.Scripts.UI.Panel
{
    public class ShopItemsPanel : View.View
    {
        [field: SerializeField] public Transform ItemContent { get; private set; }
        
        [SerializeField] private Button _backSceneButton;
        
        private ITweenAnimationService _tweenAnimationService;
        private IShopService _shopService;
        private ICurrencyService _currencyService;
        private IPlayerService _playerService;
        
        private List<ShopItemView> _itemViews;

        public event Action OnBackToSceneButtonPressed;

        [Inject]
        private void Construct(
            ITweenAnimationService tweenAnimationService,
            IShopService shopService,
            ICurrencyService currencyService,
            IPlayerService playerService)
        {
            _tweenAnimationService = tweenAnimationService;
            _shopService = shopService;
            _currencyService = currencyService;
            _playerService = playerService;
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
            foreach (var itemView in _itemViews)
            {
                itemView.OnButtonClicked += ApplyPurchase;
            }
            
            _playerService.Player.InputController.LockPlayerMovement();
            
            _tweenAnimationService.AnimateScale(transform);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);
            _playerService.Player.InputController.UnlockPlayerMovement();
        }

        public void GetItemViews(List<ShopItemView> itemViews)
        {
            _itemViews = itemViews;
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

        private void ApplyPurchase(ItemData itemData,  ShopItemView itemView)
        {
            if (itemData.Price > _currencyService.Gold)
                return;
            
            _currencyService.SpendGold(itemData.Price);
            
            switch (itemData.Type)
            {
                case ItemType.HealthPotion:
                    // newAttributeData = _healthAttributes[YG2.saves.HealthAttributeNumber];
                    break;
                case ItemType.SpeedPotion:
                    // newAttributeData = _damageAttributes[YG2.saves.DamageAttributeNumber];
                    break;
                case ItemType.Meat:
                    // newAttributeData = _armorAttributes[YG2.saves.ArmorAttributeNumber];
                    break;
            }
            
            itemView.Set(itemData);
            
            YG2.SaveProgress();
            _currencyService.SaveGold();
        }
    }
}