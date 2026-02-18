using UnityEngine;

namespace _Project.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        public Health Health { get; private set; }
        public bool CanFollow { get; private set; }
        
        private void Awake()
        {
            Health = GetComponent<Health>();
        }
        
        public void ChangeFollowEnemyState(bool canFollow)
        {
            CanFollow = canFollow;
        }
    }
}
