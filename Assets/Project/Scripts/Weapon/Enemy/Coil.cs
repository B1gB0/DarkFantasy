using Cysharp.Threading.Tasks;
using Project.Scripts.Audio.Sounds;
using Project.Scripts.Effects;
using Project.Scripts.Player;
using UnityEngine;

namespace Project.Scripts.Weapon.Enemy
{
    public class Coil : Weapon
    {
        private const float Radius = 4f;
        private const float OffsetHeight = 0.3f;

        private readonly Collider[] _results = new Collider[10];

        [SerializeField] private LayerMask _playerLayerMask;

        public override void Attack()
        {
            var position = Target.position;
            position.y += OffsetHeight;
            PerformSphereDamageNonAlloc(position, Radius, _playerLayerMask, Damage);
            ParticleEffectsService.PlayEffect(ParticleType.SoulCoil, position);
            AudioSoundsService.PlaySound(SoundsType.ExplosionSoulSound).Forget();
        }

        private void PerformSphereDamageNonAlloc(Vector3 center, float radius, LayerMask playerLayer, float damage)
        {
            int count = Physics.OverlapSphereNonAlloc(center, radius, _results, playerLayer);
            for (int i = 0; i < count; i++)
            {
                if (_results[i].TryGetComponent<PlayerHitBox>(out var hitBox))
                {
                    hitBox.HandleEnemyAttack(damage);
                }
            }
        }
    }
}