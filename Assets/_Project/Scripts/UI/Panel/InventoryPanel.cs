using System;
using System.Collections.Generic;
using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using Cysharp.Threading.Tasks;
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
        private const int MinValue = 0;
        
        [SerializeField] private Button _backSceneButton;
        [SerializeField] private Button _equipButton;
        
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _name;
        
        [SerializeField] private List<InventoryItemView> _itemViews;
        
        private ITweenAnimationService _tweenAnimationService;
        private IPlayerService _playerService;
        private IInventoryService _inventoryService;
        private IShopService _shopService;
        private AudioSoundsService _audioSoundsService;
        
        private ItemData _equippedItem;
        
        public event Action OnBackToSceneButtonPressed;
        
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
        
        private void Start()
        {
            _equipButton.interactable = false;
            
            foreach (var itemView in _itemViews)
            {
                itemView.OnSelectButtonPressed += SelectItem;
            }
            
            _equipButton.onClick.AddListener(OnEquippedButtonClicked);
            
            SetDescription();
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
            
            _equipButton.onClick.RemoveListener(OnEquippedButtonClicked);
            
            transform.DOKill();
        }
        
        public override void Show()
        {
            _playerService.Player.InputController.LockPlayerMovement();

            ResetSelection();
            
            int index = MinValue;
            foreach (var item in YG2.saves.InventoryItems)
            {
                if (index >= _itemViews.Count)
                    break;

                ItemType type = item.Key;
                int count = item.Value;

                ItemData itemData = GetItemDataByType(type);
                if (itemData == null || count <= MinValue)
                    continue;
                
                _itemViews[index].gameObject.SetActive(true);
                _itemViews[index].Set(itemData, count);
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

        private void OnEquippedButtonClicked()
        {
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();
            _inventoryService.EquipItem(_equippedItem.Type);
        }

        private void SelectItem(ItemType type, InventoryItemView  selectedItemView)
        {
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();
            
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
            
            _equippedItem = itemData;
        }
        
        private void ResetSelection()
        {
            _equipButton.interactable = false;
            
            foreach (var itemView in _itemViews)
                itemView.HideHover();
            
            SetDescription();
        }

        private void SetDescription(ItemData data = null)
        {
            if (data != null)
            {
                _name.gameObject.SetActive(true);
                
                _name.text = YG2.lang switch
                {
                    LocalizationCode.Ru => data.NameRu,
                    LocalizationCode.En => data.NameEn,
                    LocalizationCode.Tr => data.NameTr,
                    _ => _name.text
                };
                
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
                _name.gameObject.SetActive(false);
                
                _descriptionText.text = YG2.lang switch
                {
                    LocalizationCode.Ru => "Предмет не выбран",
                    LocalizationCode.En => "The item is not selected",
                    LocalizationCode.Tr => "Öğe seçilmedi",
                    _ => _descriptionText.text
                };
            }
        }

        private ItemData GetItemDataByType(ItemType type)
        {
            return _shopService.GetItemDataByType(type);
        }
    }
}