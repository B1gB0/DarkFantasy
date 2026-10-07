using System;
using _Project.Scripts.Characteristics;
using UnityEngine;

namespace _Project.Scripts.DataBase.Data
{
    [Serializable]
    public class CharacteristicsLocalizationData
    {
        [SerializeField] private CharacteristicType _type;
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;
        
        public CharacteristicType Type => _type;
        public string NameRu => _nameRu;
        public string NameEn => _nameEn;
        public string NameTr => _nameTr;
    }
}