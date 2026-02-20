using _Project.Scripts.Characteristics;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public Health Health { get; private set; }

        public PlayerCharacteristics PlayerCharacteristics { get; private set; }

        public bool CanFollow { get; private set; }
        
        private void OnDestroy()
        {
            Health.TargetHealthChanged -= PlayerCharacteristics.SaveTargetHealth;
        }

        public void Construct(PlayerCharacteristics playerCharacteristics)
        {
            PlayerCharacteristics = playerCharacteristics;
            Health.TargetHealthChanged += PlayerCharacteristics.SaveTargetHealth;
        }

        public void ChangeFollowEnemyState(bool canFollow)
        {
            CanFollow = canFollow;
        }
    }
}