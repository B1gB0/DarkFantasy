using System;
using _Project.Scripts.Enemy;
using UnityEngine;

namespace _Project.Scripts.DataBase.Data
{
    [Serializable]
    public class EnemyData
    {
        [SerializeField] private EnemyType _type;
        [SerializeField] private float _health;
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _damage;
        [SerializeField] private float _fireRate;
        [SerializeField] private float _rangeAttack;
        [SerializeField] private int _experience;
        [SerializeField] private int _score;
        [SerializeField] private int _stopDistance;
        [SerializeField] private float _armor;
        [SerializeField] private float _speedProjectile;
        
        public EnemyType Type => _type;
        public float Health => _health;
        public float Speed => _speed;
        public float RotationSpeed => _rotationSpeed;
        public float Damage => _damage;
        public float FireRate => _fireRate;
        public float RangeAttack => _rangeAttack;
        public int Experience => _experience;
        public int Score => _score;
        public float StopDistance => _stopDistance;
        public float Armor => _armor;
        public float SpeedProjectile => _speedProjectile;
    }
}