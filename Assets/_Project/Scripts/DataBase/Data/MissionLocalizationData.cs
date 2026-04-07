using System;
using UnityEngine;

namespace _Project.Scripts.DataBase.Data
{
    [Serializable]
    public class MissionLocalizationData
    {
        [SerializeField] private string _id;
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;

        public string Id => _id;
        public string NameRu => _nameRu;
        public string NameEn => _nameEn;
        public string NameTr => _nameTr;
    }
}