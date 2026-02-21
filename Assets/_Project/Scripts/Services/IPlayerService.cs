using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public interface IPlayerService : IService
    {
        public Player.Player Player { get; }
        public PlayerData GetPlayerDataByType(PlayerType type);
        public Player.Player CreatePlayerByPrefab(Player.Player playerPrefab, Vector3 spawnPoint);
        public PlayerCharacteristics InitPlayerCharacteristics();
    }
}