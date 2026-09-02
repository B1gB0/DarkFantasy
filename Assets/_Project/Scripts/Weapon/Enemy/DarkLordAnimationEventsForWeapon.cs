using UnityEngine;

namespace _Project.Scripts.Weapon.Enemy
{
    public class DarkLordAnimationEventsForWeapon : MonoBehaviour
    {
        [SerializeField] private Coil _coil;
        [SerializeField] private FireballSpell _fireballSpell;

        public void OnCoilAttack() => _coil.Attack();
        public void FireballAttack() => _fireballSpell.Attack();
    }
}