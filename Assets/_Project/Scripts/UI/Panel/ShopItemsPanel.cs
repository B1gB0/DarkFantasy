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
        private ICurrencyService _currencyService;
        private IPlayerService _playerService;
        private IInventoryService _inventoryService;
        
        private List<ShopItemView> _itemViews;

        public event Action OnBackToSceneButtonPressed;

        [Inject]
        private void Construct(
            ITweenAnimationService tweenAnimationService,
            IShopService shopService,
            ICurrencyService currencyService,
            IPlayerService playerService,
            IInventoryService inventoryService)
        {
            _tweenAnimationService = tweenAnimationService;
            _currencyService = currencyService;
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
            foreach (var itemView in _itemViews)
            {
                itemView.SetCurrencyColor(_currencyService.Gold);
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
            
            _inventoryService.AddItem(itemData.Type);
            
            YG2.SaveProgress();
            _currencyService.SaveGold();
        }
    }
}