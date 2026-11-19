using System;
using System.Collections.Generic;
using NorskaLib.Spreadsheets;
using Project.Scripts.DataBase.Data;

namespace Project.Scripts.DataBase
{
    [Serializable]
    public class SpreadsheetContent
    {
        [SpreadsheetPage("Players")] public List<PlayerData> Players;
        [SpreadsheetPage("Enemies")] public List<EnemyData> Enemies;

        [SpreadsheetPage("CharacteristicsLocalization")]
        public List<CharacteristicsLocalizationData> CharacteristicsLocalizationData;

        [SpreadsheetPage("GraveyardSceneLevels")]
        public List<SceneLevelData> GraveyardSceneLevels;

        [SpreadsheetPage("BanditCampSceneLevels")]
        public List<SceneLevelData> BanditCampSceneLevels;

        [SpreadsheetPage("CastleSceneLevels")] public List<SceneLevelData> CastleSceneLevels;

        [SpreadsheetPage("MissionLocalization")]
        public List<MissionLocalizationData> MissionsLocalization;

        [SpreadsheetPage("ItemsData")] public List<ItemData> ItemsData;

        [SpreadsheetPage("PlayerAttributeLevelsData")]
        public List<PlayerAttributeLevelData> PlayerAttributeLevelData;

        [SpreadsheetPage("UILocalization")] public List<UILocalizationData> UILocalizationData;
    }
}