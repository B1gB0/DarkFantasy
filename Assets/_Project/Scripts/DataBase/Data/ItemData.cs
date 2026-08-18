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

        public string Id => _id;
        public ItemType Type => _type;
        public float Value => _value;
        public int Price => _price;
    }
}