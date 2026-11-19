using Cysharp.Threading.Tasks;
using Project.Scripts.Audio.Sounds;
using Project.Scripts.Projectile;

namespace Project.Scripts.Weapon.Enemy
{
    public class FireballSpell : GenericWeapon<Fireball>
    {
        public override void Attack()
        {
            AudioSoundsService.PlaySound(SoundsType.Fireball).Forget();
            base.Attack();
        }
    }
}