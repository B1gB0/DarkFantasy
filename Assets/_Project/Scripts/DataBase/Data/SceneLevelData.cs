using System;
using UnityEngine;

namespace _Project.Scripts.DataBase.Data
{
    [Serializable]
    public class SceneLevelData
    {
        [SerializeField] private int _number;
        [SerializeField] private string _sceneName;

        public int Number => _number;
        public string SceneName => _sceneName;
    }
}