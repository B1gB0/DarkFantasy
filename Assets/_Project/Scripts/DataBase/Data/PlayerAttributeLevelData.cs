using System;
using _Project.Scripts.Characteristics;
using UnityEngine;

namespace _Project.Scripts.DataBase.Data
{
    [Serializable]
    public class PlayerAttributeLevelData
    {
        [SerializeField] private string _id;
        [SerializeField] private CharacteristicType _type;
        [SerializeField] private float _value;
        [SerializeField] private float _price;

        public string Id => _id;
        public CharacteristicType Type => _type;
        public float Value => _value;
        public float Price => _price;
    }
}