using System;
using System.Collections.Generic;
using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.Panel
{
    public class InventoryPanel : View.View
    {
        private const int MinValue = 0;

        [SerializeField] private Button _backSceneButton;

        [SerializeField] private List<InventoryItemView> _itemViews;
        [SerializeField] private DescriptionItemView _descriptionItemView;
        [SerializeField] private List<SlotView> _slotViews;

        private ITweenAnimationService _tweenAnimationService;
        private IPlayerService _playerService;
        private IInventoryService _inventoryService;
        private IShopService _shopService;
        private AudioSoundsService _audioSoundsService;

        public event Action OnBackToSceneButtonPressed;

        public ItemData EquippedItem { get; private set; }

        [Inject]
        private void Construct(
            ITweenAnimationService tweenAnimationService,
            IShopService shopService,
            IPlayerService playerService,
            IInventoryService inventoryService,
            AudioSoundsService audioSoundsService)
        {
            _tweenAnimationService = tweenAnimationService;
            _shopService = shopService;
            _playerService = playerService;
            _inventoryService = inventoryService;
            _audioSoundsService = audioSoundsService;
        }

        private void OnEnable()
        {
            _backSceneButton.onClick.AddListener(MoveBackToScene);
        }

        private void Start()
        {
            foreach (var itemView in _itemViews)
            {
                itemView.OnSelectButtonPressed += SelectItem;
            }
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

            foreach (var itemView in _itemViews)
                itemView.Set();

            // 1. Сначала экипировка (уникальные экземпляры)
            int index = MinValue;

            var equipment = _inventoryService.Equipment;
            for (int i = 0; i < equipment.Count; i++)
            {
                if (index >= _itemViews.Count) break;

                var instance = equipment[i];
                if (instance == null) continue;

                var data = GetItemDataByType(instance.Type);
                if (data == null) continue;

                _itemViews[index].gameObject.SetActive(true);
                _itemViews[index].SetEquipment(data, instance);
                index++;
            }

            // 2. Затем расходники (стакаются)
            foreach (var kvp in YG2.saves.Consumables)
            {
                if (index >= _itemViews.Count) break;

                var type = kvp.Key;
                int count = kvp.Value;
                if (count <= MinValue) continue;

                var data = GetItemDataByType(type);
                if (data == null) continue;

                _itemViews[index].gameObject.SetActive(true);
                _itemViews[index].SetConsumable(data, count);
                index++;
            }

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

        private void SelectItem(InventoryItemView selectedItemView)
        {
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();

            foreach (var itemView in _itemViews)
                itemView.HideHover();

            selectedItemView.ShowHover();
            
            if (selectedItemView.HasEquipmentInstance)
            {
                var data = selectedItemView.ItemData;
                var instance = selectedItemView.EquipmentInstance;
                _descriptionItemView.SetEquipment(data, instance);
            }
            else
            {
                var data = selectedItemView.ItemData;
                _descriptionItemView.SetConsumable(data);
            }

            _descriptionItemView.Show();

            EquippedItem = selectedItemView.ItemData;
        }

        private ItemData GetItemDataByType(ItemType type)
            => _shopService.GetItemDataByType(type);
    }
}