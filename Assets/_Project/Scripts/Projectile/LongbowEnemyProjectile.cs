using UnityEngine;

namespace _Project.Scripts.Projectile
{
    public class LongbowEnemyProjectile : EnemyProjectile
    {
        private const float DefaultDirectionY = 0f;

        public override void SetDirection(Vector3 targetPosition)
        {
            Direction = (targetPosition - Transform.position).normalized;
            Transform.LookAt(targetPosition);
            Direction.y = DefaultDirectionY;
            Direction = Direction.normalized;
        }
    }
}