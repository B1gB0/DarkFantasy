using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public interface IPlayerService : IService
    {
        public Player.Core.Player Player { get; }
        public PlayerData GetPlayerDataByType(PlayerType type);
        public Player.Core.Player CreatePlayerByPrefab(Player.Core.Player playerPrefab, Vector3 spawnPoint);
        public PlayerCharacteristics InitPlayerCharacteristics();
    }
}