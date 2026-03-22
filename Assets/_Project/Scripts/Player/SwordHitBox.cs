using _Project.Scripts.Enemy.StateMachine.Behaviour.States;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class SwordHitbox : MonoBehaviour
    {
        public bool CanDamage { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!CanDamage)
                return;

            if (other.TryGetComponent(out Enemy.Enemy enemy))
            {
                enemy.EnemyStateMachine.SwitchState<HitState>();
                enemy.Health.TakeDamage(5f, enemy.Armor);
            }
        }
    }
}