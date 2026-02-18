using _Project.Scripts.Weapon;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(Longbow))]
    public class SkeletonRanger : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public EnemyPatrolComponent EnemyPatrolComponent { get; private set; }
        [field: SerializeField] public AttackComponent AttackComponent { get; private set; }
        [field: SerializeField] public FollowComponent FollowComponent { get; private set; }
        
        public Longbow Longbow { get; private set; }
        
        private void Awake()
        {
            Longbow = GetComponent<Longbow>();
        }
    }
}