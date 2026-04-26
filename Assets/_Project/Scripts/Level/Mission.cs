using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.DataBase.InitDataSO;
using UnityEngine;

namespace _Project.Scripts.Level
{
    [CreateAssetMenu(menuName = "Missions/New Mission")]
    public class Mission : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public List<LevelInitData> Maps { get; private set; } = new();
        [field: SerializeField] public string NameRu { get; private set; }
        [field: SerializeField] public string NameEn { get; private set; }
        [field: SerializeField] public string NameTr { get; private set; }
        
        [field: SerializeField] public Sprite Image { get; private set; }

        public void SetData(MissionLocalizationData data)
        {
            Id = data.Id;
            NameRu = data.NameRu;
            NameEn = data.NameEn;
            NameTr = data.NameTr;
        }
    }
}