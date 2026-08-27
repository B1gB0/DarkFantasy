using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;

namespace _Project.Scripts.Characteristics
{
    [Serializable]
    public class PlayerCharacteristics
    {
        private const float PercentFactor = 100f;
        
        public float MaxHealth;
        public float TargetHealth;
        public float Armor;
        public float Damage;
        public float MoveSpeed;
        public float RotationSpeed;

        [NonSerialized] 
        private List<SpeedModifier> _speedModifiers = new();
        [NonSerialized] 
        private float _baseMoveSpeed;

        private IPlayerService _playerService;

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
            _playerService.Player.Health.LoadHealth(MaxHealth, TargetHealth);
            _speedModifiers?.Clear();
            _baseMoveSpeed = MoveSpeed;
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
        
        public void AddSpeedModifier(float value, float duration, bool isMultiplier = false)
        {
            var modifier = new SpeedModifier(value, isMultiplier, duration);
            _speedModifiers.Add(modifier);
            RemoveSpeedModifierAfterDelay(modifier, duration).Forget();
        }
        
        public void ClearSpeedModifiers()
        {
            _speedModifiers?.Clear();
        }
        
        public float GetCurrentMoveSpeed()
        {
            float result = _baseMoveSpeed;
            
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

        private async UniTaskVoid RemoveSpeedModifierAfterDelay(SpeedModifier modifier, float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            _speedModifiers.Remove(modifier);
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