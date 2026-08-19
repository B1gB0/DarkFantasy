using System;
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
        [SerializeField] private int _price;
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;

        public string Id => _id;
        public ItemType Type => _type;
        public float Value => _value;
        public int Price => _price;
        public string NameRu => _nameRu;
        public string NameEn => _nameEn;
        public string NameTr => _nameTr;
    }
}