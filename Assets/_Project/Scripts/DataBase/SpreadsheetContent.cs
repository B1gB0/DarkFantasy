using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using NorskaLib.Spreadsheets;

namespace _Project.Scripts.DataBase
{
    [Serializable]
    public class SpreadsheetContent
    {
        [SpreadsheetPage("Players")] public List<PlayerData> Players;
        [SpreadsheetPage("Enemies")] public List<EnemyData> Enemies;
    }
}