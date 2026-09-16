using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using _Project.Scripts.Services;
using UnityEngine;
using YG;

namespace _Project.Scripts.Characteristics
{
    [Serializable]
    public class PlayerCharacteristics
    {
        private const int MinValue = 0;
        private const int CorrectFactor = 1;
        
        private const float PercentFactor = 100f;
        
        public float MaxHealth;
        public float TargetHealth;
        public float Armor;
        public float Damage;
        public float MoveSpeed;
        public float RotationSpeed;

        [SerializeField] private List<SpeedModifier> _speedModifiers = new();
        [SerializeField] private HealingModifier _healingModifier;
        
        [NonSerialized] private float _baseMoveSpeed;
        [NonSerialized] private IPlayerService _playerService;
        
        public IReadOnlyList<SpeedModifier> SpeedModifiers => _speedModifiers;
        
        public void SaveHealingState(HealingModifier healingModifier) => _healingModifier = healingModifier;

        public void SetStartingData(PlayerData data)
        {
            MaxHealth = data.Health;
            TargetHealth = data.Health;
            MoveSpeed = data.MoveSpeed;
            RotationSpeed = data.RotationSpeed;
            Armor = data.Armor;
            Damage = data.Damage;

            _baseMoveSpeed = data.MoveSpeed;
        }

        public void SetCharacteristics(IPlayerService playerService)
        {
            _playerService = playerService;
            
            for (int i = _speedModifiers.Count - CorrectFactor; i >= MinValue; i--)
            {
                if (!_speedModifiers[i].Timer.IsActive)
                    _speedModifiers.RemoveAt(i);
            }
            
            _playerService.Player.Health.LoadHealth(MaxHealth, TargetHealth);
            _playerService.Player.Health.RestoreHealingState(_healingModifier);
            
            _baseMoveSpeed = MoveSpeed;
        }
        
        public void Tick(float deltaTime)
        {
            if (_speedModifiers == null || _speedModifiers.Count == MinValue)
                return;

            for (int i = _speedModifiers.Count - CorrectFactor; i >= MinValue; i--)
            {
                if (_speedModifiers[i].Tick(deltaTime))
                    _speedModifiers.RemoveAt(i);
            }
        }

        public void SaveTargetHealth(float targetHealth)
        {
            TargetHealth = targetHealth;
        }

        public void ApplyImprovement(CharacteristicType type, float factor)
        {
            switch (type)
            {
                case CharacteristicType.Health:
                    YG2.saves.HealthAttributeNumber++;
                    IncreaseHealth(factor);
                    break;
                case CharacteristicType.Armor:
                    YG2.saves.ArmorAttributeNumber++;
                    IncreaseArmor(factor);
                    break;
                case CharacteristicType.Damage:
                    YG2.saves.DamageAttributeNumber++;
                    IncreaseDamage(factor);
                    break;
            }
        }
        
        public bool AddSpeedModifier(float value, float duration, bool isMultiplier = false)
        {
            _speedModifiers ??= new List<SpeedModifier>();
            
            for (int i = _speedModifiers.Count - CorrectFactor; i >= MinValue; i--)
            {
                if (!_speedModifiers[i].Timer.IsActive)
                    _speedModifiers.RemoveAt(i);
            }

            if (_speedModifiers.Count > MinValue)
                return false;

            _speedModifiers.Add(new SpeedModifier(value, isMultiplier, duration));
            return true;
        }
        
        public void ClearSpeedModifiers()
        {
            _speedModifiers?.Clear();
        }
        
        public float GetCurrentMoveSpeed()
        {
            float result = _baseMoveSpeed;
            if (_speedModifiers == null) return result;

            foreach (var mod in _speedModifiers)
            {
                if (!mod.IsMultiplier)
                    result += mod.Value;
            }

            foreach (var mod in _speedModifiers)
            {
                if (mod.IsMultiplier)
                    result += _baseMoveSpeed * mod.Value / PercentFactor;
            }

            return result;
        }

        private void IncreaseHealth(float healthValue)
        {
            MaxHealth += healthValue;
            _playerService.Player.Health.ImproveHealth(healthValue);
        }

        private void IncreaseArmor(float armorValue)
        {
            PlayerData data = _playerService.GetPlayerDataByType(PlayerType.CommonHero);
            Armor = data.Armor + armorValue;
        }

        private void IncreaseDamage(float damageValue)
        {
            PlayerData data = _playerService.GetPlayerDataByType(PlayerType.CommonHero);
            Damage = data.Damage + damageValue;
        }
    }
}