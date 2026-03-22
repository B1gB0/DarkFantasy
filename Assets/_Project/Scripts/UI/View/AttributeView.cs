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
        [SerializeField] private List<Sprite> _icons;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _iconAttribute;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private TMP_Text _price;

        public void Set(CharacteristicsLocalizationData data)
        {
            switch (data.Type)
            {
                case CharacteristicType.Health:
                    _iconAttribute.sprite = _icons[0];
                    SetLocalization(data);
                    break;
                case CharacteristicType.Damage:
                    _iconAttribute.sprite = _icons[1];
                    SetLocalization(data);
                    break;
                case CharacteristicType.Armor:
                    _iconAttribute.sprite = _icons[2];
                    SetLocalization(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
    }
}