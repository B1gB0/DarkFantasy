using System;
using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Services;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class AttributeView : View
    {
        [SerializeField] private Button _attributeButton;
        [SerializeField] private List<Sprite> _icons;
        [SerializeField] private Image _iconAttribute;
        
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private TMP_Text _nextValue;
        [SerializeField] private TMP_Text _currentValue;

        private PlayerAttributeLevelData _currentData;
        private IPlayerService _playerService;
        
        public event Action<PlayerAttributeLevelData, AttributeView> OnButtonClicked;
        
        [Inject]
        private void Construct(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        
        private void OnEnable()
        {
            _attributeButton.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _attributeButton.onClick.RemoveListener(OnButtonClick);
        }

        public void SetCurrencyColor(int gold)
        {
            if(_currentData == null) return;
            if(_currentData.Price > gold)
                _price.color = Colors.GetColor(ColorName.RedCurrencyColor);
            else
                _price.color = Colors.GetColor(ColorName.BlackUIColor);
        }

        public void Set(CharacteristicsLocalizationData localizationData, PlayerAttributeLevelData attributeData)
        {
            switch (localizationData.Type)
            {
                case CharacteristicType.Health:
                    _currentValue.text = _playerService.Player.PlayerCharacteristics.MaxHealth.ToString();
                    _iconAttribute.sprite = _icons[0];
                    break;
                case CharacteristicType.Damage:
                    _currentValue.text = _playerService.Player.PlayerCharacteristics.Damage.ToString();
                    _iconAttribute.sprite = _icons[1];
                    break;
                case CharacteristicType.Armor:
                    _currentValue.text = _playerService.Player.PlayerCharacteristics.Armor.ToString();
                    _iconAttribute.sprite = _icons[2];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetLocalization(localizationData);

            _currentData = attributeData;
            
            _nextValue.text = '+' + attributeData.Value.ToString();
            _price.text = attributeData.Price.ToString();
        }

        private void SetLocalization(CharacteristicsLocalizationData data)
        {
            _title.text = YG2.lang switch
            {
                LocalizationCode.Ru => data.NameRu,
                LocalizationCode.En => data.NameEn,
                LocalizationCode.Tr => data.NameTr,
                _ => _title.text
            };
        }

        private void OnButtonClick()
        {
            OnButtonClicked?.Invoke(_currentData, this);
        }
    }
}