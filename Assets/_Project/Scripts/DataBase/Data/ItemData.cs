using System;
using _Project.Scripts.Characteristics;
using _Project.Scripts.Items;
using UnityEngine;

namespace _Project.Scripts.DataBase.Data
{
    [Serializable]
    public class ItemData
    {
        [SerializeField] private string _id;
        [SerializeField] private ItemType _type;
        [SerializeField] private float _value;
        [SerializeField] private CharacteristicType _characteristicType;
        [SerializeField] private float _value2;
        [SerializeField] private CharacteristicType _characteristicType2;
        [SerializeField] private float _value3;
        [SerializeField] private CharacteristicType _characteristicType3;
        [SerializeField] private int _price;
        [SerializeField] private float _duration;
        [SerializeField] private bool _isMultiplier;
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;
        [SerializeField] private string _descriptionRu;
        [SerializeField] private string _descriptionEn;
        [SerializeField] private string _descriptionTr;
        [SerializeField] private ItemKind _kind;
        [SerializeField] private EquipmentType _slot;
        [SerializeField] private ItemRarity _rarity;
        [SerializeField] private bool _isSold;

        public string Id => _id;
        public ItemType Type => _type;
        public float Value => _value;
        public CharacteristicType CharacteristicType => _characteristicType;
        public float Value2 => _value2;
        public CharacteristicType CharacteristicType2 => _characteristicType2;
        public float Value3 => _value3;
        public CharacteristicType CharacteristicType3 => _characteristicType3;
        public int Price => _price;
        public float Duration => _duration;
        public bool IsMultiplier => _isMultiplier;
        public string NameRu => _nameRu;
        public string NameEn => _nameEn;
        public string NameTr => _nameTr;
        public string DescriptionRu => _descriptionRu;
        public string DescriptionEn => _descriptionEn;
        public string DescriptionTr => _descriptionTr;
        public ItemKind Kind => _kind;
        public EquipmentType Slot => _slot;
        public ItemRarity Rarity => _rarity;
        public bool IsSold => _isSold;
    }
}