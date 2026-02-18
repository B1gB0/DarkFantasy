using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class PlayerService : MonoBehaviour, IPlayerService
    {
        private readonly Dictionary<PlayerType, PlayerData> _playersData = new ();
        
        private IDataBaseService _dataBaseService;
        
        public bool IsInitiated { get; private set; }
        public Player.Player Player { get; private set; }
        
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
        
        public PlayerData GetPlayerDataByType(PlayerType type)
        {
            return _playersData[type];
        }

        public Player.Player CreatePlayerByPrefab(Player.Player playerPrefab, Vector3 spawnPoint)
        {
            Player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);

            return Player;
        }
    }
}