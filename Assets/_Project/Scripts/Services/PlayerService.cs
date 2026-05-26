using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.Services
{
    public class PlayerService : MonoBehaviour, IPlayerService
    {
        private const float MinValue = 0f;
        
        private readonly Dictionary<PlayerType, PlayerData> _playersData = new ();
        
        private IDataBaseService _dataBaseService;
        
        public bool IsInitiated { get; private set; }
        public Player.Core.Player Player { get; private set; }
        
        private Container _container;

        [Inject]
        public void Construct(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }
        
        public CinemachineFreeLook FreeLookCamera { get; private set; }

        public UniTask Init()
        {
            if (IsInitiated)
                return UniTask.CompletedTask;

            foreach (var player in _dataBaseService.Content.Players)
            {
                _playersData.TryAdd(player.Type, player);
            }

            IsInitiated = true;

            return UniTask.CompletedTask;
        }
        
        public PlayerCharacteristics InitPlayerCharacteristics(PlayerData data)
        {
            var characteristics = YG2.saves.PlayerCharacteristics;

            if (characteristics != null)
            {
                characteristics.SetCharacteristics(this);
            }
            else
            {
                characteristics = new PlayerCharacteristics();
                characteristics.SetStartingData(data);
                characteristics.SetCharacteristics(this);
            }

            YG2.saves.PlayerCharacteristics = characteristics;

            return characteristics;
        }
        
        public PlayerData GetPlayerDataByType(PlayerType type)
        {
            return _playersData[type];
        }

        public Player.Core.Player CreatePlayerByPrefab(Player.Core.Player playerPrefab, Vector3 spawnPoint)
        {
            Player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
            GameObjectInjector.InjectObject(Player.gameObject, _container);

            return Player;
        }
        
        public void SpawnPlayer()
        {
            Player.gameObject.SetActive(true);

            if (Player.Health.TargetHealth <= MinValue)
            {
                Player.Health.SetHealthValue(Player.Health.MaxHealth);
            }
        }

        public void GetSceneObjects(Container container, CinemachineFreeLook freeLookCamera)
        {
            _container = container;
            FreeLookCamera = freeLookCamera;
        }
        
        public void GetJoystickWithAttackButton(Joystick joystick, Button attackButton, Button rollButton)
        {
            Player.InputController.GetJoystickWithAttackButton(joystick, attackButton, rollButton);
        }
    }
}