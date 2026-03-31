using _Project.Scripts.Weapon;
using _Project.Scripts.Weapon.Enemy;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class Priest : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public MagicSpellOfPriest MagicSpell { get; private set; }
    }
}