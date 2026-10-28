using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.Items;
using _Project.Scripts.Level;

namespace YG
{
    public partial class SavesYG
    {
        public int Gold;
        public int AcumulatedScore;
        public int ExperiencePointsValue;

        public int HealthAttributeNumber;
        public int DamageAttributeNumber;
        public int ArmorAttributeNumber;

        public bool IsFirstLaunch = true;

        public bool IsMediumDifficultyUnlock;
        public bool IsHardDifficultyUnlock;

        public bool IsAttributeShopVisited;
        public bool IsInventoryShopVisited;
        public bool IsMissionPanelVisited;
        public bool IsCampfireVisited;

        public string EquippedWeaponId;
        public string EquippedArmorId;
        public string EquippedRingId;
        
        public List<string> CompletedLow = new();
        public List<string> CompletedMedium = new();
        public List<string> CompletedHigh = new();

        public Dictionary<ItemType, int> Consumables = new();
        public List<Item> Equipment = new();

        public ItemType EquippedItemType;

        public LevelDifficulty CurrentDifficulty;

        public PlayerCharacteristics PlayerCharacteristics;

        public List<string> stringKeys = new List<string>();
        public List<string> stringValues = new List<string>();

        public List<string> floatKeys = new List<string>();
        public List<float> floatValues = new List<float>();

        public List<string> intKeys = new List<string>();
        public List<int> intValues = new List<int>();
        
        public List<string> GetCompletedMissions(LevelDifficulty difficulty)
        {
            return difficulty switch
            {
                LevelDifficulty.Low    => CompletedLow,
                LevelDifficulty.Medium => CompletedMedium,
                LevelDifficulty.High   => CompletedHigh,
                _ => CompletedLow,
            };
        }

        public bool IsMissionCompleted(string missionId, LevelDifficulty difficulty)
        {
            return GetCompletedMissions(difficulty).Contains(missionId);
        }
        
        public void MarkMissionCompleted(string missionId, LevelDifficulty difficulty)
        {
            var list = GetCompletedMissions(difficulty);
            if (list.Contains(missionId)) return;

            list.Add(missionId);
            YG2.SaveProgress();
        }
    }
}