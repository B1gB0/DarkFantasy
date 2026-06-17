using System;
using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
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
        [SerializeField] private TMP_Text _value;
        [SerializeField] private TMP_Text _price;

        private PlayerAttributeLevelData _currentData;
        
        public event Action<PlayerAttributeLevelData, AttributeView> OnButtonClicked;
        
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
                    _iconAttribute.sprite = _icons[0];
                    SetLocalization(localizationData);
                    break;
                case CharacteristicType.Damage:
                    _iconAttribute.sprite = _icons[1];
                    SetLocalization(localizationData);
                    break;
                case CharacteristicType.Armor:
                    _iconAttribute.sprite = _icons[2];
                    SetLocalization(localizationData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currentData = attributeData;

            _value.text = attributeData.Value.ToString();
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