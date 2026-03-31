using System;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using _Project.Scripts.Services;
using YG;

namespace _Project.Scripts.Characteristics
{
    [Serializable]
    public class PlayerCharacteristics
    {
        private const float MoveSpeedFactor = 1f;
        
        public float MaxHealth;
        public float TargetHealth;
        public float Armor;
        public float Damage;
        public float MoveSpeed;
        public float RotationSpeed;

        private IPlayerService _playerService;
        
        private float _moveSpeed;
        private float _baseMoveSpeed;

        public void SetStartingData(PlayerData data)
        {
            MaxHealth = data.Health;
            TargetHealth = data.Health;
            MoveSpeed = data.MoveSpeed;
            RotationSpeed = data.RotationSpeed;
            Armor = data.Armor;
            Damage = data.Damage;

            _moveSpeed = data.MoveSpeed;
            _baseMoveSpeed = data.MoveSpeed;
        }

        public void SetCharacteristics(IPlayerService playerService)
        {
            _playerService = playerService;
            _playerService.Player.Health.LoadHealth(MaxHealth, TargetHealth);
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
                // case CharacteristicType.DiggingSpeed:
                //     IncreaseDiggingSpeedFactor(factor);
                //     break;
                // case CharacteristicType.MoveSpeed:
                //     IncreaseMoveSpeed(factor);
                //     break;
            }
        }

        public void UpdateCurrentSpeed()
        {
            // _moveSpeed = _baseMoveSpeed * (MoveSpeedFactor + _playerService.Player.GetCurrentModifier());
            // ChangeMovableComponentSpeed(_moveSpeed);
        }

        private void SetDiggingSpeed(float diggingSpeedFactor)
        {
            // PlayerData data = _playerService.GetPlayerDataByType(PlayerActorType.CommonStardiver);
            //
            // float newDiggingSpeed = data.DiggingSpeed - (data.DiggingSpeed * diggingSpeedFactor);
            // _diggingSpeed = newDiggingSpeed;
            //
            // _playerService.PlayerActor.MiningToolActor.ChangeDiggingSpeed(newDiggingSpeed);
        }

        private void SetMoveSpeed(float moveSpeedFactor)
        {
            PlayerData data = _playerService.GetPlayerDataByType(PlayerType.CommonHero);

            _baseMoveSpeed = data.MoveSpeed + (data.MoveSpeed * moveSpeedFactor);

            UpdateCurrentSpeed();
        }

        private void ChangeMovableComponentSpeed(float newMoveSpeed)
        {
            _moveSpeed = newMoveSpeed;
            // _playerService.ChangeMoveSpeed(_moveSpeed);
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

        private void IncreaseDiggingSpeedFactor(float diggingSpeedFactor)
        {
            SetDiggingSpeed(diggingSpeedFactor);
        }

        private void IncreaseMoveSpeed(float moveSpeedFactor)
        {
            SetMoveSpeed(moveSpeedFactor);
        }
    }
}