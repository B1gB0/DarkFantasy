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
using YG;

namespace _Project.Scripts.Services
{
    public class PlayerService : MonoBehaviour, IPlayerService
    {
        private readonly Dictionary<PlayerType, PlayerData> _playersData = new ();
        
        private IDataBaseService _dataBaseService;
        private Container _container;

        [Inject]
        public void Construct(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }
        
        public bool IsInitiated { get; private set; }
        public Player.Player Player { get; private set; }
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

        public Player.Player CreatePlayerByPrefab(Player.Player playerPrefab, Vector3 spawnPoint)
        {
            Player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
            GameObjectInjector.InjectObject(Player.gameObject, _container);

            return Player;
        }

        public void GetSceneObjects(Container container, CinemachineFreeLook freeLookCamera)
        {
            _container = container;
            FreeLookCamera = freeLookCamera;
        }
    }
}