using System;
using UnityEngine;

namespace _Project.Scripts.Weapon.Enemy
{
    public class EnemyWeaponHitBox : MonoBehaviour
    {
        public event Action<Scripts.Player.Core.Player> OnHitPlayer;
        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.TryGetComponent(out Scripts.Player.Core.Player player))
            {
                OnHitPlayer?.Invoke(player);
            }
        }
    }
}