using _Project.Scripts.Weapon;
using _Project.Scripts.Weapon.Enemy;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(Longbow))]
    public class SkeletonRanger : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }

        public Longbow Longbow { get; private set; }
        
        private void Awake()
        {
            Longbow = GetComponent<Longbow>();
        }
    }
}