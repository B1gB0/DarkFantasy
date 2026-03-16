using System;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.DataBase.Data
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private PlayerType _type;
        [SerializeField] private float _health;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _armor;

        public PlayerType Type => _type;
        public float Health => _health;
        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float Armor => _armor;
    }
}