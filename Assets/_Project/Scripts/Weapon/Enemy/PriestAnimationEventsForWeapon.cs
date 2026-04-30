using UnityEngine;

namespace _Project.Scripts.Weapon.Enemy
{
    public class PriestAnimationEventsForWeapon : MonoBehaviour
    {
        [SerializeField] private Coil _coil;
        [SerializeField] private Omni _omni;
        [SerializeField] private FireballSpell _fireballSpell;

        public void OnCoilAttack() => _coil.Attack();
        public void OmniAttack() => _omni.Attack();
        public void FireballAttack() => _fireballSpell.Attack();
    }
}