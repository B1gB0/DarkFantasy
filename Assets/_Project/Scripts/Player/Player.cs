using UnityEngine;

namespace _Project.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public Health Health { get; private set; }
        
        public bool CanFollow { get; private set; }

        public void ChangeFollowEnemyState(bool canFollow)
        {
            CanFollow = canFollow;
        }
    }
}
