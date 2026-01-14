using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private int _health = 100;

        public void TakeDamage(int damage)
        {
            if(_health <= 0) return;
            
            _health -= damage;
        }
    }
}