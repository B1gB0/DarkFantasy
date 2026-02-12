using UnityEngine;

namespace _Project.Scripts.DataBase.InitDataSO
{
    [CreateAssetMenu(menuName = "InitData/PlayerInitData")]
    public class PlayerInitData : InitData
    {
        [field: SerializeField] public Player.Player PlayerPrefab { get; private set; }
    }
}