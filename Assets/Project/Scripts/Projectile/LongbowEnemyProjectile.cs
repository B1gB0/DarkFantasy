using UnityEngine;

namespace Project.Scripts.Projectile
{
    public class LongbowEnemyProjectile : EnemyProjectile
    {
        public override void SetDirection(Vector3 targetPosition)
        {
            base.SetDirection(targetPosition);
            Transform.LookAt(targetPosition);
        }
    }
}