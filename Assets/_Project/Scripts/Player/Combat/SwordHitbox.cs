using _Project.Scripts.Enemy.StateMachine.Behaviour.States;
using UnityEngine;
using YG;

namespace _Project.Scripts.Player.Combat
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
                enemy.Health.TakeDamage(YG2.saves.PlayerCharacteristics.Damage, enemy.Armor);
            }
        }
    }
}
