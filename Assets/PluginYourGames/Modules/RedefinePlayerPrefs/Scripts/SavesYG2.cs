using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Player;

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
        public bool IsBanditCampUnlock;
        public bool IsCastleUnlock;

        public bool IsAttributeShopVisited;
        public bool IsInventoryShopVisited;
        public bool IsMissionPanelVisited;

        public ItemType EquippedItemType;
        
        public PlayerCharacteristics PlayerCharacteristics;
        public Dictionary<ItemType, int> InventoryItems = new ();
        
        public List<string> stringKeys = new List<string>();
        public List<string> stringValues = new List<string>();

        public List<string> floatKeys = new List<string>();
        public List<float> floatValues = new List<float>();

        public List<string> intKeys = new List<string>();
        public List<int> intValues = new List<int>();
    }
}
