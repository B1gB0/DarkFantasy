using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Effects;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Weapon.Enemy
{
    public class Omni : Weapon
    {
        private const float Radius = 6f;
        private const float Force = 150f;
        private const float OffsetHeight = 1f;
        
        [SerializeField] private LayerMask _playerLayerMask;
        
        private readonly Collider[] _results = new Collider[10];
        
        public override void Attack()
        {
            var position = transform.position;
            position.y += OffsetHeight;
            
            ParticleEffectsService.PlayEffect(ParticleType.MagicExplosion, position);
            AudioSoundsService.PlaySound(SoundsType.ExplosionMagic).Forget();
            
            int count = Physics.OverlapSphereNonAlloc(position, Radius, _results, _playerLayerMask);
            for (int i = 0; i < count; i++)
            {
                if (_results[i].TryGetComponent<Scripts.Player.Core.Player>(out var player))
                {
                    player.Health.TakeDamage(Damage, player.PlayerCharacteristics.Armor);
                    
                    if (player.Rigidbody != null)
                    {
                        Vector3 direction = (player.transform.position - position).normalized;
                        direction.y = 0f;
                        player.Rigidbody.AddForce(direction * Force, ForceMode.Acceleration);
                    }
                }
            }
        }
    }
}