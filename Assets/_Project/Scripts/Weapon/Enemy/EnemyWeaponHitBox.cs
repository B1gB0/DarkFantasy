using System;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.Weapon.Enemy
{
    public class EnemyWeaponHitBox : MonoBehaviour
    {
        public event Action<PlayerHitBox> OnHitPlayer;

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.TryGetComponent(out PlayerHitBox player))
            {
                OnHitPlayer?.Invoke(player);
            }
        }
    }
}