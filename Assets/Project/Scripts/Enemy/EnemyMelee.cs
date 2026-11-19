using Project.Scripts.Weapon.Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Scripts.Enemy
{
    public class EnemyMelee : Enemy
    {
        [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public MeleeWeapon MeleeWeapon { get; private set; }
    }
}