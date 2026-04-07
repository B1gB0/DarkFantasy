using System.Collections.Generic;
using _Project.Scripts.Level;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class MissionService : MonoBehaviour, IService
    {
        private const int DefaultNumberLevel = 0;

        private readonly Dictionary<int, string> _graveyardSceneLevels = new();

        private IDataBaseService _dataBaseService;

        [Inject]
        public void Construct(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }

        [field: SerializeField] public List<Mission> Missions { get; private set; } = new();

        public Mission CurrentOperation { get; private set; }
        public int CurrentNumberLevel { get; private set; }
        public bool IsInitiated { get; private set; }

        public UniTask Init()
        {
            if (IsInitiated)
                return UniTask.CompletedTask;

            foreach (var mission in Missions)
            {
                foreach (var operationData in _dataBaseService.Content.MissionsLocalization)
                {
                    if (mission.Id == operationData.Id)
                    {
                        mission.SetData(operationData);
                    }
                }
            }

            foreach (var graveyardSceneLevel in _dataBaseService.Content.GraveyardSceneLevels)
            {
                _graveyardSceneLevels.Add(graveyardSceneLevel.Number, graveyardSceneLevel.SceneName);
            }

            IsInitiated = true;

            return UniTask.CompletedTask;
        }

        public void SetCurrentOperation(int index)
        {
            CurrentOperation = Missions[index];
            CurrentNumberLevel = DefaultNumberLevel;
        }

        public void SetCurrentNumberLevel(int numberLevel)
        {
            CurrentNumberLevel = numberLevel;
        }

        public string GetSceneNameByCurrentNumber()
        {
            return GetSceneNameByNumber(CurrentNumberLevel);
        }

        public string GetSceneNameByNumber(int number)
        {
            return CurrentOperation.Id switch
            {
                Game.Constant.Missions.Graveyard => _graveyardSceneLevels[number],
                _ => null
            };
        }
    }
}