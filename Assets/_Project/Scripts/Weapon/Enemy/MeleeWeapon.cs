using UnityEngine;

namespace _Project.Scripts.Weapon.Enemy
{
    public class MeleeWeapon : Weapon
    {
        [SerializeField] private EnemyWeaponHitBox _hitBox;

        private void Start()
        {
            _hitBox.OnHitPlayer += HandleAttack;
            EndAttack();
        }

        private void OnDestroy()
        {
            _hitBox.OnHitPlayer -= HandleAttack;
        }

        public override void Attack()
        {
            _hitBox.gameObject.SetActive(true);
        }

        private void EndAttack()
        {
            _hitBox.gameObject.SetActive(false);
        }

        private void HandleAttack(Scripts.Player.Core.Player player)
        {
            player.Health.TakeDamage(Damage, player.PlayerCharacteristics.Armor);
            EndAttack();
        }
    }
}