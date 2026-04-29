using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Projectile;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Weapon.Enemy
{
    public class MagicSpellOfPriest : GenericEnemyWeapon<MagicBall>
    {
        public override void Attack()
        {
            AudioSoundsService.PlaySound(SoundsType.Fireball).Forget();
            base.Attack();
        }
    }
}