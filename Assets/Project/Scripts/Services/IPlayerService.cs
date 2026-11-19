using Cinemachine;
using Project.Scripts.Characteristics;
using Project.Scripts.DataBase.Data;
using Project.Scripts.Player;
using Reflex.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Services
{
    public interface IPlayerService : IService
    {
        public CinemachineFreeLook FreeLookCamera { get; }
        public Player.Core.Player Player { get; }
        
        public PlayerData GetPlayerDataByType(PlayerType type);
        
        public Player.Core.Player CreatePlayerByPrefab(Player.Core.Player playerPrefab, Vector3 spawnPoint);
        
        public PlayerCharacteristics InitPlayerCharacteristics(PlayerData data);
        
        public void SpawnPlayer(bool isRevive = false);
        
        public void GetSceneObjects(Container container, CinemachineFreeLook freeLookCamera);

        public void GetButtons(
            Joystick moveJoystick,
            Joystick cameraJoystick,
            Button attackButton,
            Button rollButton,
            Button inventoryButton,
            Button pickUpButton);
    }
}