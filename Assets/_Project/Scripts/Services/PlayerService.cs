using System;
using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
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
        private IShopService _shopService;
        private IInventoryService _inventoryService;
        
        public bool IsInitiated { get; private set; }
        public Player.Core.Player Player { get; private set; }
        
        private Container _container;

        [Inject]
        public void Construct(
            IDataBaseService dataBaseService,
            IShopService shopService,
            IInventoryService inventoryService)
        {
            _dataBaseService = dataBaseService;
            _shopService = shopService;
            _inventoryService =  inventoryService;


            _inventoryService.OnEquippedItem += RecalculateEquipment;
            _inventoryService.OnUnEquippedItem += RecalculateEquipment;
        }
        
        public CinemachineFreeLook FreeLookCamera { get; private set; }
        
        private void Update()
        {
            if (YG2.saves == null) return;

            var characteristics = YG2.saves.PlayerCharacteristics;
            if (characteristics == null) return;

            characteristics.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _inventoryService.OnEquippedItem -= RecalculateEquipment;
            _inventoryService.OnUnEquippedItem -= RecalculateEquipment;
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
        
        public PlayerCharacteristics InitPlayerCharacteristics(PlayerData data)
        {
            var characteristics = YG2.saves.PlayerCharacteristics;
            
            if (characteristics == null || characteristics.MaxHealth <= MinValue)
            {
                characteristics ??= new PlayerCharacteristics();
                characteristics.SetStartingData(data);
            }

            characteristics.SetCharacteristics(this);
            YG2.saves.PlayerCharacteristics = characteristics;

            RecalculateEquipment();

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
                Player.Health.SetHealthValue(Player.Health.MaxHealth);
            
            var characteristics = YG2.saves.PlayerCharacteristics;
            characteristics?.BindToPlayer(Player);

            Player.StateMachine.SwitchState(StateId.Idle);
        }

        public void GetSceneObjects(Container container, CinemachineFreeLook freeLookCamera)
        {
            _container = container;
            FreeLookCamera = freeLookCamera;
        }
        
        public void GetButtons(
            Joystick moveJoystick,
            Joystick cameraJoystick,
            Button attackButton,
            Button rollButton,
            Button inventoryButton,
            Button equippedItemButton)
        {
            Player.InputController.GetButtons(
                moveJoystick,
                cameraJoystick,
                attackButton,
                rollButton,
                inventoryButton,
                equippedItemButton);
        }
        
        private void RecalculateEquipment()
        {
            var characteristics = YG2.saves.PlayerCharacteristics;
            if (characteristics == null) return;

            var equipped = CollectEquippedItems();
            characteristics.RecalculateEquipmentBonuses(equipped);
            
            if (Player != null && Player.Health != null)
                Player.Health.SetMaxHealth(characteristics.GetTotalMaxHealth());
        }
        
        private List<ItemData> CollectEquippedItems()
        {
            var result = new List<ItemData>(3);
            AddIfEquipped(result, YG2.saves.EquipedWeaponType);
            AddIfEquipped(result, YG2.saves.EquipedArmorType);
            AddIfEquipped(result, YG2.saves.EquipedRingType);
            return result;
        }

        private void AddIfEquipped(List<ItemData> list, ItemType type)
        {
            if (type == ItemType.None) return;
            var data = _shopService.GetItemDataByType(type);
            if (data != null) list.Add(data);
        }
    }
}