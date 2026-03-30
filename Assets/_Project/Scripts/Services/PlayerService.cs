using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using YG;

namespace _Project.Scripts.Services
{
    public class PlayerService : MonoBehaviour, IPlayerService
    {
        private readonly Dictionary<PlayerType, PlayerData> _playersData = new ();
        
        private IDataBaseService _dataBaseService;
        
        public bool IsInitiated { get; private set; }
        public Player.Core.Player Player { get; private set; }
        
        [Inject]
        public void Construct(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }
        
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
        
        public PlayerCharacteristics InitPlayerCharacteristics()
        {
            var characteristics = YG2.saves.PlayerCharacteristics;

            if (characteristics != null)
            {
                characteristics.SetCharacteristics();
            }
            else
            {
                characteristics = new PlayerCharacteristics(this);
                characteristics.SetStartingCharacteristics(GetPlayerDataByType(PlayerType.CommonHero));
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

            return Player;
        }
    }
}