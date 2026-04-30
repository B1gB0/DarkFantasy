using UnityEngine;

namespace _Project.Scripts.Weapon
{
    public abstract class GenericWeapon<T> : Weapon
        where T : Projectile.Projectile
    {
        [SerializeField] private Transform _shootPoint;

        private T _projectile;
        private ObjectPool<T> _projectilePool;
        private float _speedProjectile;

        public override void Attack()
        {
            _projectile = _projectilePool.GetFreeElement();
            _projectile.transform.position = _shootPoint.position;
            _projectile.SetCharacteristics(Damage, _speedProjectile);
            _projectile.SetDirection(Target.position);
            _projectile.GetServices(ParticleEffectsService, AudioSoundsService);
        }

        public void SetProjectile(ObjectPool<T> projectilePool, float speedProjectile)
        {
            _projectilePool = projectilePool;
            _speedProjectile = speedProjectile;
        }
    }
}