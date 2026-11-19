using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Scripts.Player
{
    public class EnemyDetector : MonoBehaviour
    {
        private readonly HashSet<Enemy.Enemy> _enemiesInRange = new();

        private float _closestEnemyDistanceSqr;

        private void OnTriggerEnter(Collider otherCollider)
        {
            if (!otherCollider.TryGetComponent(out Enemy.Enemy enemy))
                return;

            _enemiesInRange.Add(enemy);

            enemy.Die += OnEnemyDie;
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            if (!otherCollider.TryGetComponent(out Enemy.Enemy enemy))
                return;

            _enemiesInRange.Remove(enemy);

            enemy.Die -= OnEnemyDie;
        }

        private void OnDestroy()
        {
            foreach (var enemy in _enemiesInRange.Where(enemyAlienActor => enemyAlienActor != null))
            {
                enemy.Die -= OnEnemyDie;
            }

            _enemiesInRange.Clear();
        }

        private void OnEnemyDie(Enemy.Enemy enemy)
        {
            _enemiesInRange.Remove(enemy);
            enemy.Die -= OnEnemyDie;
        }
    }
}