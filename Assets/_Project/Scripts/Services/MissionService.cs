using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<int, string> _banditCampSceneLevels = new();
        private readonly Dictionary<int, string> _castleSceneLevels = new();

        private IDataBaseService _dataBaseService;

        [Inject]
        public void Construct(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }

        [field: SerializeField] public List<Mission> Missions { get; private set; } = new();

        public Mission CurrentMission { get; private set; }
        public int CurrentNumberLevel { get; private set; }
        public bool IsInitiated { get; private set; }

        public UniTask Init()
        {
            if (IsInitiated)
                return UniTask.CompletedTask;

            foreach (var mission in Missions)
            {
                foreach (var missionData in _dataBaseService.Content.MissionsLocalization)
                {
                    if (mission.Id == missionData.Id)
                    {
                        mission.SetData(missionData);
                    }
                }
            }

            foreach (var graveyardSceneLevel in _dataBaseService.Content.GraveyardSceneLevels)
            {
                _graveyardSceneLevels.Add(graveyardSceneLevel.Number, graveyardSceneLevel.SceneName);
            }
            
            foreach (var banditCampSceneLevel in _dataBaseService.Content.BanditCampSceneLevels)
            {
                _banditCampSceneLevels.Add(banditCampSceneLevel.Number, banditCampSceneLevel.SceneName);
            }
            
            foreach (var castleSceneLevel in _dataBaseService.Content.CastleSceneLevels)
            {
                _castleSceneLevels.Add(castleSceneLevel.Number, castleSceneLevel.SceneName);
            }

            IsInitiated = true;

            return UniTask.CompletedTask;
        }

        public void SetCurrentMission(string id)
        {
            foreach (var mission in Missions.Where(mission => id == mission.Id))
            {
                CurrentMission = mission;
            }

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
            return CurrentMission.Id switch
            {
                Game.Constant.Missions.Graveyard => _graveyardSceneLevels[number],
                Game.Constant.Missions.BanditCamp => _banditCampSceneLevels[number],
                Game.Constant.Missions.Castle => _castleSceneLevels[number],
                _ => null
            };
        }
    }
}