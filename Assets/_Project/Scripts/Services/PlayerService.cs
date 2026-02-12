using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Player;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

namespace _Project.Scripts.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly Dictionary<PlayerType, PlayerData> _playersData = new ();
        
        private IDataBaseService _dataBaseService;
        
        public bool IsInitiated { get; private set; }
        
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
    }
}