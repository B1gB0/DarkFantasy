using System;
using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Services;
using _Project.Scripts.UI.Panel;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class DescriptionItemView : View
    {
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _backButton;

        private AudioSoundsService _audioSoundsService;
        private IInventoryService _inventoryService;
        private ItemData _currentItem;
        
        public event Action OnEquippedItem;

        [Inject]
        private void Construct(AudioSoundsService audioSoundsService, IInventoryService inventoryService)
        {
            _audioSoundsService = audioSoundsService;
            _inventoryService = inventoryService;
        }
        
        private void OnEnable()
        {
            _backButton.onClick.AddListener(Deactivate);
        }

        private void Start()
        {
            _equipButton.onClick.AddListener(OnEquippedButtonClicked);
        }
        
        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(Deactivate);
        }

        private void OnDestroy()
        {
            _equipButton.onClick.RemoveListener(OnEquippedButtonClicked);
        }
        
        public void SetDescription(ItemData data)
        {
            _currentItem = data;
            
            _name.gameObject.SetActive(true);

            _name.text = YG2.lang switch
            {
                LocalizationCode.Ru => data.NameRu,
                LocalizationCode.En => data.NameEn,
                LocalizationCode.Tr => data.NameTr,
                _ => _name.text
            };

            _description.text = YG2.lang switch
            {
                LocalizationCode.Ru => data.DescriptionRu,
                LocalizationCode.En => data.DescriptionEn,
                LocalizationCode.Tr => data.DescriptionTr,
                _ => _description.text
            };
        }

        private void OnEquippedButtonClicked()
        {
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();
            _inventoryService.EquipConsumableItem(_currentItem.Type);
            _inventoryService.EquipItem(_currentItem.Type);
            
            OnEquippedItem?.Invoke();
        }
    }
}