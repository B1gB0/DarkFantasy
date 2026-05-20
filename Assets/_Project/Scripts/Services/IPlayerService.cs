using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using Cinemachine;
using Reflex.Core;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Services
{
    public interface IPlayerService : IService
    {
        public CinemachineFreeLook FreeLookCamera { get; }
        public Player.Core.Player Player { get; }
        public PlayerData GetPlayerDataByType(PlayerType type);
        public Player.Core.Player CreatePlayerByPrefab(Player.Core.Player playerPrefab, Vector3 spawnPoint);
        public PlayerCharacteristics InitPlayerCharacteristics(PlayerData data);
        public void GetSceneObjects(Container container, CinemachineFreeLook freeLookCamera);
        public void GetJoystickWithAttackButton(Joystick joystick, Button attackButton);
    }
}