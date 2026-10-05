using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
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
        
        private const float BaseAttackSpeedMultiplier = 1f;
        private const float PercentFactor = 100f;

        public float MaxHealth;
        public float TargetHealth;
        public float Armor;
        public float Damage;
        public float MoveSpeed;
        public float RotationSpeed;
        private float _healthRegenInterval = 1f;

        [SerializeField] private List<SpeedModifier> _speedModifiers;
        [SerializeField] private HealingModifier _healingModifier;

        [NonSerialized] private float _baseMoveSpeed;
        [NonSerialized] private IPlayerService _playerService;
        [NonSerialized] private IShopService _shopService;
        [NonSerialized] private IProgressionService _progressionService;

        [NonSerialized] private float _equipmentArmorBonus;
        [NonSerialized] private float _equipmentDamageBonus;
        [NonSerialized] private float _equipmentMaxHealthBonus;
        [NonSerialized] private float _equipmentHealthRegenBonus;
        [NonSerialized] private float _equipmentMoveSpeedBonus;
        [NonSerialized] private float _equipmentAttackSpeedBonus;
        [NonSerialized] private float _equipmentCritDamageBonus;
        [NonSerialized] private float _equipmentReverseDamageBonus;

        public IReadOnlyList<SpeedModifier> SpeedModifiers => _speedModifiers;
        public HealingModifier HealingModifier => _healingModifier;

        public float GetTotalArmor() => Armor + _equipmentArmorBonus;
        public float GetTotalDamage() => Damage + _equipmentDamageBonus;
        public float GetTotalMaxHealth() => MaxHealth + _equipmentMaxHealthBonus;
        public float GetTotalMoveSpeed() => GetCurrentMoveSpeed() + _equipmentMoveSpeedBonus;

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

        public void SetCharacteristics(
            IPlayerService playerService,
            IShopService shopService,
            IProgressionService progressionService)
        {
            _playerService = playerService;
            _shopService = shopService;
            _progressionService = progressionService;
            _speedModifiers ??= new List<SpeedModifier>();

            for (int i = _speedModifiers.Count - CorrectFactor; i >= MinValue; i--)
            {
                if (!_speedModifiers[i].Timer.IsActive)
                    _speedModifiers.RemoveAt(i);
            }

            _baseMoveSpeed = MoveSpeed;
        }
        
        public void RecalculateEquipmentBonuses(
            IReadOnlyList<Item> equippedItems)
        {
            _equipmentArmorBonus = 0f;
            _equipmentDamageBonus = 0f;
            _equipmentMaxHealthBonus = 0f;
            _equipmentHealthRegenBonus = 0f;
            _equipmentMoveSpeedBonus = 0f;
            _equipmentAttackSpeedBonus = 0f;
            _equipmentCritDamageBonus = 0f;
            _equipmentReverseDamageBonus = 0f;
            _healthRegenInterval = 0f;

            foreach (var instance in equippedItems)
            {
                if (instance == null) continue;

                var template = _shopService.GetItemDataByType(instance.Type);
                if (template == null) continue;

                float mul = _progressionService.GetItemMultiplier(instance.Difficulty);
                
                if (template.BonusType != BonusType.None)
                    ApplyBonus(template.BonusType, template.Value * mul);
                
                if (template.BonusType2 != BonusType.None)
                    ApplyBonus(template.BonusType2, template.Value2 * mul);
                
                if (template.BonusType3 != BonusType.None)
                    ApplyBonus(template.BonusType3, template.Value3 * mul);
            }
        }

        private void ApplyBonus(BonusType type, float value)
        {
            switch (type)
            {
                case BonusType.Armor: 
                    _equipmentArmorBonus += value;
                    break;
                case BonusType.Damage: 
                    _equipmentDamageBonus += value;
                    break;
                case BonusType.MaxHealth: 
                    _equipmentMaxHealthBonus += value;
                    break;
                case BonusType.HealthRegen: 
                    _equipmentHealthRegenBonus += value;
                    break;
                case BonusType.MoveSpeed:  
                    _equipmentMoveSpeedBonus += value;
                    break;
                case BonusType.AttackSpeed:
                    _equipmentAttackSpeedBonus += value;
                    break;
                case BonusType.CritDamage:
                    _equipmentCritDamageBonus += value;
                    break;
                case BonusType.ReverseDamage:
                    _equipmentReverseDamageBonus += value;
                    break;
                case BonusType.None:
                    break;
                default:
                    Debug.LogWarning($"[Characteristics] Unknown bonus: {type}");
                    break;
            }
        }

        public void BindToPlayer(Player.Core.Player player)
        {
            if (player == null || player.Health == null)
                return;

            player.Health.LoadHealth(GetTotalMaxHealth(), TargetHealth);
            player.Health.BindCharacteristics(this);
        }

        public void Tick(float deltaTime)
        {
            if (_speedModifiers != null && _speedModifiers.Count > MinValue)
            {
                for (int i = _speedModifiers.Count - CorrectFactor; i >= MinValue; i--)
                {
                    if (_speedModifiers[i].Tick(deltaTime))
                    {
                        _speedModifiers.RemoveAt(i);
                    }
                }
            }

            if (_healingModifier != null && _healingModifier.Tick(deltaTime))
                _healingModifier = null;
        }
        
        public float GetHealthRegenAmount()
        {
            return _equipmentHealthRegenBonus;
        }

        public float GetHealthRegenInterval()
        {
            return _healthRegenInterval;
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

        public void ClearHealing()
        {
            _healingModifier = null;
        }

        public bool TryStartHealing(float totalAmount, float duration)
        {
            if (duration <= MinValue)
                return false;

            if (_healingModifier != null && _healingModifier.Timer.IsActive)
                return false;

            _healingModifier = new HealingModifier(totalAmount, duration);
            return true;
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
        
        public float GetAttackSpeedMultiplier()
        {
            return BaseAttackSpeedMultiplier + _equipmentAttackSpeedBonus / PercentFactor;
        }

        private void IncreaseHealth(float healthValue)
        {
            MaxHealth += healthValue;
            _playerService.Player.Health.ImproveHealth(healthValue);
        }

        private void IncreaseArmor(float armorValue)
        {
            Armor += armorValue;
        }

        private void IncreaseDamage(float damageValue)
        {
            Damage += damageValue;
        }
    }
}